const User = require("../models/user");
const Procurement = require("../models/procurement");
const Labeled_Procurement = require("../models/labeledProcurement");
const jwt = require("jsonwebtoken");
const { hashPassword, comparePassword } = require("../helpers/auth");
const bodyParser = require("body-parser");

const test = (req, res) => {
  res.json("test");
};

const registerUser = async (req, res) => {
  try {
    const { name, email, password } = req.body;

    if (!name) {
      return res.json({
        error: "name is required",
      });
    }

    if (!password || password.length < 6) {
      return res.json({
        error: "password is required and should be at least 6 characters",
      });
    }

    const exist = await User.findOne({ email });
    if (exist) {
      return res.json({ error: "Email is already taken" });
    }
    const hashedPassword = await hashPassword(password);
    const user = await User.create({ name, email, password: hashedPassword });

    return res.json(user);
  } catch (error) {
    console.log(error);
  }
};
const loginUser = async (req, res) => {
  try {
    const { email, password } = req.body;

    if (!email) {
      return res.json({ error: "Email is required" });
    }
    const user = await User.findOne({ email });
    if (!user) {
      return res.json({ error: "User is not found" });
    }

    const match = await comparePassword(password, user.password);
    if (match) {
      jwt.sign(
        { email: user.email, id: user._id, name: user.name },
        process.env.JWT_SECRET,
        {},
        (err, token) => {
          if (err) throw err;
          res.cookie("token", token).json(user);
        }
      );
    }
    if (!match) {
      res.json({ error: "password is not match" });
    }
  } catch (error) {
    console.log(error);
  }
};
const allprocurements = async (req, res) => {
  try {
    const getProcurements = await Procurement.find();
    if (!getProcurements) {
      return res.status(404).json({ msg: "procurement is not found" });
    }
    res.status(200).json(getProcurements);
  } catch (error) {
    res.status(500).json({ error: error });
  }
};

const addHistory = async (req, res) => {
  try {
    const { title, description, owner, business_areas, percentages } = req.body;
    await Labeled_Procurement.create({
      title,
      description,
      owner,
      business_areas,
      percentages,
    }).then(() => {
      return res.status(200).json({ msg: "Procurement has added to History!" });
    });
  } catch (error) {
    return res.status(404).json({ msg: "Error 404" });
  }
};

const getLabeledProcurements = async (req, res) => {
  try {
    const { owner } = await req.body;
    await Labeled_Procurement.find({
      owner,
    }).then((e) => {
      return res.status(200).json(e);
    });
  } catch (error) {
    return res.status(404).json({ msg: "Error 404" });
  }
};

module.exports = {
  test,
  registerUser,
  loginUser,
  allprocurements,
  addHistory,
  getLabeledProcurements,
};
