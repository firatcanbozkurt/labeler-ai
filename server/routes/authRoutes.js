const express = require("express");
const router = express.Router();
const cors = require("cors");
const {
  test,
  registerUser,
  loginUser,
  allprocurements,
  addHistory,
  getLabeledProcurements,
} = require("../controllers/authController");

router.use(
  cors({
    credentials: true,
    origin: "http://localhost:3000",
  })
);

router.get("/", test);
router.post("/register", registerUser);
router.post("/login", loginUser);
router.get("/getProcurements", allprocurements);
router.post("/addHistory", addHistory);
router.post("/getLabeledProcurements", getLabeledProcurements);
module.exports = router;
