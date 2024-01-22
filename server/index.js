const express = require("express");
const dotenv = require("dotenv").config();
const cors = require("cors");
const { mongoose } = require("mongoose");
const cookieParser = require("cookie-parser");

const app = express();

app.use(express.json());
app.use(cookieParser());
app.use(express.urlencoded({ extended: false }));
const bodyParser = require("body-parser");

app.use("/", require("./routes/authRoutes"));

const port = process.env.PORT;
app.listen(port, () => console.log(`Server is running on port ${port}`));

mongoose
  .connect(process.env.MONGO_URL)
  .then(() => console.log("Database connected"))
  .catch((err) => console.log("error connecting to mongo", err));
