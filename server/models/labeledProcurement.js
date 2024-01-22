const mongoose = require("mongoose");
const { Schema } = mongoose;

const labeledProcurementSchema = new Schema({
  title: String,
  description: String,
  owner: String,
  business_areas: [],
  percentages: [],
});

const LabeledProcurementModel = mongoose.model(
  "Labeled_Procurement",
  labeledProcurementSchema
);

module.exports = LabeledProcurementModel;
