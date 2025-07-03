# Production Line SCADA Templates for FT Optix

## Overview
This document provides ready-to-use templates and code examples specifically designed for production line SCADA systems using FactoryTalk Optix.

---

## 1. Common Production Line Equipment Templates

### Motor Control Template
```csharp
// NetLogic for Motor Control
using System;
using UAManagedCore;
using FTOptix.Core;
using FTOptix.HMIProject;

public class MotorControl : BaseNetLogic
{
    private IUAVariable motorStatus;
    private IUAVariable motorSpeed;
    private IUAVariable motorCurrent;
    private IUAVariable alarmStatus;
    
    public override void Start()
    {
        // Initialize motor variables
        motorStatus = LogicObject.GetVariable("Status");
        motorSpeed = LogicObject.GetVariable("Speed");
        motorCurrent = LogicObject.GetVariable("Current");
        alarmStatus = LogicObject.GetVariable("AlarmStatus");
    }
    
    [ExportMethod]
    public void StartMotor()
    {
        if (CheckInterlocks())
        {
            motorStatus.Value = 1; // Running
            Log.Info("Motor", "Motor started successfully");
        }
        else
        {
            Log.Warning("Motor", "Cannot start - Interlock active");
        }
    }
    
    [ExportMethod]
    public void StopMotor()
    {
        motorStatus.Value = 0; // Stopped
        motorSpeed.Value = 0;
        Log.Info("Motor", "Motor stopped");
    }
    
    private bool CheckInterlocks()
    {
        // Add your interlock logic here
        return true;
    }
}
```

### Conveyor System Template
```csharp
// Conveyor Control with Product Tracking
public class ConveyorControl : BaseNetLogic
{
    private IUAVariable conveyorSpeed;
    private IUAVariable productCount;
    private IUAVariable jamsDetected;
    
    public override void Start()
    {
        conveyorSpeed = LogicObject.GetVariable("Speed");
        productCount = LogicObject.GetVariable("ProductCount");
        jamsDetected = LogicObject.GetVariable("JamsDetected");
        
        // Start periodic monitoring
        PeriodicTask = new PeriodicTask(MonitorConveyor, 1000, LogicObject);
        PeriodicTask.Start();
    }
    
    private void MonitorConveyor()
    {
        // Check for jams
        var currentSpeed = (double)conveyorSpeed.Value;
        var expectedSpeed = (double)LogicObject.GetVariable("SetSpeed").Value;
        
        if (Math.Abs(currentSpeed - expectedSpeed) > 10 && expectedSpeed > 0)
        {
            // Possible jam detected
            jamsDetected.Value = (int)jamsDetected.Value + 1;
            TriggerAlarm("Conveyor jam detected");
        }
    }
    
    private void TriggerAlarm(string message)
    {
        var alarm = InformationModel.Make<Alarm>(message);
        alarm.Message = message;
        alarm.Severity = 2; // Warning
        LogicObject.Add(alarm);
    }
}
```

### Tank Level Monitoring Template
```csharp
// Tank Level Control with High/Low Alarms
public class TankLevelControl : BaseNetLogic
{
    private IUAVariable levelSensor;
    private IUAVariable fillValve;
    private IUAVariable drainValve;
    private double highLevel = 90.0;
    private double lowLevel = 10.0;
    
    public override void Start()
    {
        levelSensor = LogicObject.GetVariable("Level");
        fillValve = LogicObject.GetVariable("FillValve");
        drainValve = LogicObject.GetVariable("DrainValve");
        
        // Monitor level continuously
        PeriodicTask = new PeriodicTask(CheckLevel, 500, LogicObject);
        PeriodicTask.Start();
    }
    
    private void CheckLevel()
    {
        double currentLevel = (double)levelSensor.Value;
        
        // High level alarm
        if (currentLevel >= highLevel)
        {
            fillValve.Value = false; // Close fill valve
            CreateAlarm("High Level", AlarmSeverity.High);
        }
        
        // Low level alarm
        if (currentLevel <= lowLevel)
        {
            drainValve.Value = false; // Close drain valve
            CreateAlarm("Low Level", AlarmSeverity.Medium);
        }
        
        // Auto fill logic
        if (currentLevel < 30 && !(bool)fillValve.Value)
        {
            fillValve.Value = true;
        }
        else if (currentLevel > 70 && (bool)fillValve.Value)
        {
            fillValve.Value = false;
        }
    }
}
```

---

## 2. Production KPI Calculations

### OEE (Overall Equipment Effectiveness) Calculator
```csharp
public class OEECalculator : BaseNetLogic
{
    // OEE = Availability × Performance × Quality
    
    [ExportMethod]
    public void CalculateOEE()
    {
        var plannedTime = (double)LogicObject.GetVariable("PlannedProductionTime").Value;
        var actualRunTime = (double)LogicObject.GetVariable("ActualRunTime").Value;
        var idealCycleTime = (double)LogicObject.GetVariable("IdealCycleTime").Value;
        var totalCount = (int)LogicObject.GetVariable("TotalCount").Value;
        var goodCount = (int)LogicObject.GetVariable("GoodCount").Value;
        
        // Availability = Run Time / Planned Production Time
        double availability = actualRunTime / plannedTime * 100;
        
        // Performance = (Total Count × Ideal Cycle Time) / Run Time
        double performance = (totalCount * idealCycleTime) / actualRunTime * 100;
        
        // Quality = Good Count / Total Count
        double quality = (double)goodCount / totalCount * 100;
        
        // OEE
        double oee = (availability * performance * quality) / 10000;
        
        // Update variables
        LogicObject.GetVariable("Availability").Value = Math.Round(availability, 2);
        LogicObject.GetVariable("Performance").Value = Math.Round(performance, 2);
        LogicObject.GetVariable("Quality").Value = Math.Round(quality, 2);
        LogicObject.GetVariable("OEE").Value = Math.Round(oee, 2);
        
        // Log to database
        LogOEEToDatabase(availability, performance, quality, oee);
    }
    
    private void LogOEEToDatabase(double avail, double perf, double qual, double oee)
    {
        var db = Project.Current.Get<Store>("DataStores/ProductionDB");
        var table = db.Tables.Get<Table>("OEEHistory");
        
        var row = table.Insert();
        row.Set("Timestamp", DateTime.Now);
        row.Set("LineID", LogicObject.GetVariable("LineID").Value);
        row.Set("Availability", avail);
        row.Set("Performance", perf);
        row.Set("Quality", qual);
        row.Set("OEE", oee);
    }
}
```

### Production Rate Monitor
```csharp
public class ProductionRateMonitor : BaseNetLogic
{
    private DateTime lastCheck;
    private int lastCount;
    
    public override void Start()
    {
        lastCheck = DateTime.Now;
        lastCount = 0;
        
        // Calculate rate every minute
        PeriodicTask = new PeriodicTask(CalculateRate, 60000, LogicObject);
        PeriodicTask.Start();
    }
    
    private void CalculateRate()
    {
        var currentCount = (int)LogicObject.GetVariable("ProductCounter").Value;
        var currentTime = DateTime.Now;
        
        // Calculate time difference in hours
        double timeDiff = (currentTime - lastCheck).TotalHours;
        
        // Calculate production rate (units per hour)
        double rate = (currentCount - lastCount) / timeDiff;
        
        LogicObject.GetVariable("ProductionRate").Value = Math.Round(rate, 0);
        
        // Check if rate is below target
        var targetRate = (double)LogicObject.GetVariable("TargetRate").Value;
        if (rate < targetRate * 0.9)
        {
            CreatePerformanceAlert("Production rate below target");
        }
        
        // Update for next calculation
        lastCheck = currentTime;
        lastCount = currentCount;
    }
}
```

---

## 3. Alarm Management Templates

### Alarm Priority Configuration
```csharp
public enum AlarmPriority
{
    Critical = 1,    // Safety issues, immediate stop required
    High = 2,        // Production stop, immediate attention
    Medium = 3,      // Performance degradation
    Low = 4,         // Information only
}

public class AlarmManager : BaseNetLogic
{
    [ExportMethod]
    public void CreateProductionAlarm(string equipment, string message, int priority)
    {
        var alarm = InformationModel.Make<DigitalAlarm>($"{equipment}_Alarm");
        alarm.Message = $"[{equipment}] {message}";
        alarm.Severity = priority;
        alarm.AutoAcknowledge = priority > 3; // Auto-ack low priority
        alarm.EnableTime = DateTime.Now;
        
        // Add to active alarms
        var alarmFolder = Project.Current.Get("Model/Alarms/ActiveAlarms");
        alarmFolder.Add(alarm);
        
        // Log to database
        LogAlarmToHistory(equipment, message, priority);
        
        // Send notification if critical
        if (priority == 1)
        {
            SendCriticalAlarmNotification(equipment, message);
        }
    }
    
    private void LogAlarmToHistory(string equipment, string message, int priority)
    {
        var db = Project.Current.Get<Store>("DataStores/AlarmHistory");
        var table = db.Tables.Get<Table>("AlarmLog");
        
        var row = table.Insert();
        row.Set("Timestamp", DateTime.Now);
        row.Set("Equipment", equipment);
        row.Set("Message", message);
        row.Set("Priority", priority);
        row.Set("Acknowledged", false);
    }
}
```

---

## 4. Recipe Management Template

### Production Recipe Handler
```csharp
public class RecipeManager : BaseNetLogic
{
    [ExportMethod]
    public void LoadRecipe(string recipeName)
    {
        try
        {
            var recipeStore = Project.Current.Get<RecipeSchema>("Model/Recipes");
            var recipe = recipeStore.Get(recipeName);
            
            if (recipe != null)
            {
                // Apply recipe parameters
                ApplyRecipeParameters(recipe);
                
                // Log recipe change
                LogRecipeChange(recipeName);
                
                // Update UI
                LogicObject.GetVariable("CurrentRecipe").Value = recipeName;
                LogicObject.GetVariable("RecipeLoadStatus").Value = "Success";
            }
        }
        catch (Exception ex)
        {
            Log.Error("Recipe", $"Failed to load recipe: {ex.Message}");
            LogicObject.GetVariable("RecipeLoadStatus").Value = "Failed";
        }
    }
    
    private void ApplyRecipeParameters(Recipe recipe)
    {
        // Example: Apply temperature setpoint
        var tempSetpoint = recipe.Get<IUAVariable>("TemperatureSetpoint");
        if (tempSetpoint != null)
        {
            Project.Current.GetVariable("Model/Process/Temperature/Setpoint").Value = tempSetpoint.Value;
        }
        
        // Example: Apply speed setpoint
        var speedSetpoint = recipe.Get<IUAVariable>("ConveyorSpeed");
        if (speedSetpoint != null)
        {
            Project.Current.GetVariable("Model/Conveyor/SpeedSetpoint").Value = speedSetpoint.Value;
        }
        
        // Apply other parameters...
    }
}
```

---

## 5. Shift Management Template

### Shift Performance Tracker
```csharp
public class ShiftTracker : BaseNetLogic
{
    private readonly Dictionary<string, TimeSpan> shiftSchedule = new Dictionary<string, TimeSpan>
    {
        { "Morning", new TimeSpan(6, 0, 0) },
        { "Afternoon", new TimeSpan(14, 0, 0) },
        { "Night", new TimeSpan(22, 0, 0) }
    };
    
    [ExportMethod]
    public void EndShift()
    {
        var currentShift = GetCurrentShift();
        
        // Collect shift data
        var shiftData = new ShiftData
        {
            ShiftName = currentShift,
            EndTime = DateTime.Now,
            ProductionCount = (int)LogicObject.GetVariable("ShiftProductCount").Value,
            GoodCount = (int)LogicObject.GetVariable("ShiftGoodCount").Value,
            DowntimeMinutes = (double)LogicObject.GetVariable("ShiftDowntime").Value,
            OEE = (double)LogicObject.GetVariable("ShiftOEE").Value
        };
        
        // Save to database
        SaveShiftReport(shiftData);
        
        // Reset counters for next shift
        ResetShiftCounters();
        
        // Generate shift report
        GenerateShiftReport(shiftData);
    }
    
    private string GetCurrentShift()
    {
        var currentTime = DateTime.Now.TimeOfDay;
        
        if (currentTime >= shiftSchedule["Morning"] && currentTime < shiftSchedule["Afternoon"])
            return "Morning";
        else if (currentTime >= shiftSchedule["Afternoon"] && currentTime < shiftSchedule["Night"])
            return "Afternoon";
        else
            return "Night";
    }
}
```

---

## 6. Energy Monitoring Template

### Energy Consumption Tracker
```csharp
public class EnergyMonitor : BaseNetLogic
{
    private double lastEnergyReading = 0;
    private DateTime lastReadTime;
    
    public override void Start()
    {
        lastReadTime = DateTime.Now;
        
        // Monitor energy every 15 minutes
        PeriodicTask = new PeriodicTask(MonitorEnergy, 900000, LogicObject);
        PeriodicTask.Start();
    }
    
    private void MonitorEnergy()
    {
        var currentReading = (double)LogicObject.GetVariable("EnergyMeter/TotalKWH").Value;
        var currentTime = DateTime.Now;
        
        // Calculate consumption
        double consumption = currentReading - lastEnergyReading;
        double timeDiffHours = (currentTime - lastReadTime).TotalHours;
        double powerKW = consumption / timeDiffHours;
        
        // Update displays
        LogicObject.GetVariable("CurrentPowerKW").Value = Math.Round(powerKW, 2);
        LogicObject.GetVariable("ConsumptionKWH").Value = Math.Round(consumption, 2);
        
        // Check for abnormal consumption
        var normalPower = (double)LogicObject.GetVariable("NormalPowerKW").Value;
        if (powerKW > normalPower * 1.2)
        {
            CreateEnergyAlert("Abnormal power consumption detected");
        }
        
        // Log to database
        LogEnergyData(currentTime, powerKW, consumption);
        
        // Update for next reading
        lastEnergyReading = currentReading;
        lastReadTime = currentTime;
    }
}
```

---

## 7. Quality Control Template

### Quality Inspection Station
```csharp
public class QualityControl : BaseNetLogic
{
    [ExportMethod]
    public void RecordInspection(int productId, bool passed, string defectCode = "")
    {
        var db = Project.Current.Get<Store>("DataStores/QualityDB");
        var table = db.Tables.Get<Table>("InspectionResults");
        
        var row = table.Insert();
        row.Set("Timestamp", DateTime.Now);
        row.Set("ProductID", productId);
        row.Set("Passed", passed);
        row.Set("DefectCode", defectCode);
        row.Set("Inspector", Session.User.BrowseName);
        row.Set("LineID", LogicObject.GetVariable("LineID").Value);
        
        // Update counters
        if (passed)
        {
            IncrementCounter("PassCount");
        }
        else
        {
            IncrementCounter("RejectCount");
            UpdateDefectTracking(defectCode);
        }
        
        // Calculate quality rate
        CalculateQualityRate();
    }
    
    private void UpdateDefectTracking(string defectCode)
    {
        if (string.IsNullOrEmpty(defectCode)) return;
        
        var defectVar = LogicObject.GetVariable($"DefectCounts/{defectCode}");
        if (defectVar != null)
        {
            defectVar.Value = (int)defectVar.Value + 1;
        }
    }
    
    private void CalculateQualityRate()
    {
        var passCount = (int)LogicObject.GetVariable("PassCount").Value;
        var totalCount = (int)LogicObject.GetVariable("TotalInspected").Value;
        
        if (totalCount > 0)
        {
            double qualityRate = (double)passCount / totalCount * 100;
            LogicObject.GetVariable("QualityRate").Value = Math.Round(qualityRate, 2);
        }
    }
}
```

---

## 8. Maintenance Schedule Template

### Preventive Maintenance Tracker
```csharp
public class MaintenanceScheduler : BaseNetLogic
{
    public override void Start()
    {
        // Check maintenance schedule daily
        PeriodicTask = new PeriodicTask(CheckMaintenanceSchedule, 86400000, LogicObject);
        PeriodicTask.Start();
    }
    
    private void CheckMaintenanceSchedule()
    {
        var equipment = Project.Current.Get("Model/Equipment").Children;
        
        foreach (var item in equipment)
        {
            var lastMaintenance = item.GetVariable("LastMaintenance");
            var maintenanceInterval = item.GetVariable("MaintenanceInterval");
            
            if (lastMaintenance != null && maintenanceInterval != null)
            {
                var lastDate = (DateTime)lastMaintenance.Value;
                var intervalDays = (int)maintenanceInterval.Value;
                
                var nextDue = lastDate.AddDays(intervalDays);
                var daysUntilDue = (nextDue - DateTime.Now).Days;
                
                // Create notification if due soon
                if (daysUntilDue <= 7)
                {
                    CreateMaintenanceNotification(item.BrowseName, daysUntilDue);
                }
                
                // Update status
                item.GetVariable("MaintenanceStatus").Value = 
                    daysUntilDue <= 0 ? "Overdue" : 
                    daysUntilDue <= 7 ? "Due Soon" : "OK";
            }
        }
    }
}
```

---

## Usage Instructions

1. **Import Templates**: Copy the relevant code into your FT Optix NetLogic scripts
2. **Customize Variables**: Update variable paths to match your project structure
3. **Configure Parameters**: Adjust thresholds, intervals, and limits for your specific needs
4. **Test Thoroughly**: Test each template in a development environment first
5. **Document Changes**: Keep track of customizations for future reference

These templates provide a solid foundation for common production line SCADA requirements and can be extended based on your specific needs.