import React from "react";
import axios from "axios";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-hot-toast";
function Register() {
  const navigate = useNavigate();
  const [data, setData] = useState({
    name: "",
    email: "",
    password: "",
  });

  const registerUser = async (e) => {
    e.preventDefault();
    const { name, email, password } = data;
    try {
      const { data } = await axios.post("/register", {
        name,
        email,
        password,
      });
      if (data.error) {
        toast.error(data.error);
      } else {
        setData({ name: "", email: "", password: "" });
        toast.success("Register Successful. Login!");
        navigate("/login");
      }
    } catch (error) {
      console.log(error);
    }
  };

  const navigateLogin = () => {
    navigate("/login");
  };
  return (
    <div className="flex justify-center items-center pt-24">
      <div className=" p-8 rounded shadow-md w-full max-w-md border-2 border-white/40">
        <h2 className="text-2xl font-bold mb-6 flex justify-center">
          Register
        </h2>
        <form onSubmit={registerUser}>
          <label className="form-control w-full ">
            <div className="label">
              <span className="label-text">Name</span>
            </div>
            <input
              type="text"
              placeholder="Type here"
              className="input input-bordered w-full "
              value={data.name}
              onChange={(e) => setData({ ...data, name: e.target.value })}
            />
          </label>
          <label className="form-control w-full  my-4">
            <div className="label">
              <span className="label-text">Email</span>
            </div>
            <input
              type="text"
              placeholder="Type here"
              className="input input-bordered w-full "
              value={data.email}
              onChange={(e) => setData({ ...data, email: e.target.value })}
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
              value={data.password}
              onChange={(e) => setData({ ...data, password: e.target.value })}
            />
          </label>
          <button className="btn btn-accent w-full mt-10" type="submit">
            Register
          </button>
          <div className="w-full flex justify-between items-center mt-6">
            <p>Already have an account</p>
            <button className="btn glass" onClick={navigateLogin}>
              Login
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default Register;
