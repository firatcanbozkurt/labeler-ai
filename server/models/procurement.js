const mongoose = require("mongoose");
const {Schema} = mongoose;


const procurementSchema = new Schema({
    title: String,
    description: String,
})



const ProcurementModel = mongoose.model("Procurement", procurementSchema);

module.exports =  ProcurementModel ;