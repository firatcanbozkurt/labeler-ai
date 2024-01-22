import React, { useContext, useState } from "react";
import { UserContext } from "../context/userContext";
import { useNavigate } from "react-router-dom";
import { toast } from "react-hot-toast";
import axios from "axios";

function Login() {
  const { login, user } = useContext(UserContext);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post("/login", {
        email,
        password,
      });
      if (response.data.error) {
        toast.error(response.data.error);
      } else {
        login(response.data);
        toast.success("Login is Successful");
        navigate("/");
        console.log(user);
      }
    } catch (error) {
      toast.error("err");
    }
  };
  const navigateRegister = () => {
    navigate("/register");
  };
  return (
    <div className="flex justify-center items-center pt-24">
      <div className=" p-8 rounded shadow-md w-full max-w-md border-2 border-white/40">
        <h2 className="text-2xl font-bold mb-6 flex justify-center">Login</h2>
        <form onSubmit={handleLogin}>
          <label className="form-control w-full ">
            <div className="label">
              <span className="label-text">Email</span>
            </div>
            <input
              type="text"
              placeholder="Type here"
              className="input input-bordered w-full "
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </label>

          <label className="form-control w-full ">
            <div className="label">
              <span className="label-text">Password</span>
            </div>
            <input
              type="password"
              placeholder="Type here"
              className="input input-bordered w-full"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </label>
          <button className="btn btn-accent w-full mt-10" type="submit">
            Login
          </button>
          <div className="w-full flex justify-between items-center mt-8">
            <p>Don't have an account</p>
            <button className="btn glass" onClick={navigateRegister}>
              Register
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default Login;
