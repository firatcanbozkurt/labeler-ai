import { Routes, Route } from "react-router-dom";
import axios from "axios";
import "./App.css";
import Home from "./pages/Home";
import Register from "./pages/Register";
import Login from "./pages/Login";
import { Toaster } from "react-hot-toast";
import NavBar from "./components/NavBar";
import Label from "./pages/Label";
import History from "./pages/History";

axios.defaults.baseURL = "http://localhost:8000";
axios.defaults.withCredentials = true;
function App() {
  return (
    <>
      <NavBar />
      <Toaster position="top-center" toastOptions={{ duration: 1000 }} />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        <Route path="/label" element={<Label />} />
        <Route path="/history" element={<History />} />
      </Routes>
    </>
  );
}

export default App;
