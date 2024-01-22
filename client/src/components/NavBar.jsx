import React, { useContext } from "react";
import { UserContext } from "../context/userContext";
import { Link } from "react-router-dom";
import profile from "../public/profile.jpg";
import profile2 from "../public/3D_Person.jpg";
import logo from "../public/logo.png";
const NavBar = () => {
  const { user, logout } = useContext(UserContext);
  return (
    <div className="navbar bg-base-300 text-white">
      <div className="flex-1">
        <img
          src={logo}
          //width={200}
          className="btn btn-ghost w-[400px] h-[100px] object-cover fill py-2"
        />
      </div>
      <div className="flex-none gap-2">
        {user ? (
          <div className="dropdown dropdown-end p-4">
            <div
              tabIndex={0}
              role="button"
              className="btn btn-ghost btn-circle avatar"
            >
              <div className="w-20 rounded-full overflow-hidden">
                <img
                  alt="Tailwind CSS Navbar component"
                  src={profile2}
                  className="h-full w-full object-cover rounded-full"
                />
              </div>
            </div>
            <ul
              tabIndex={0}
              className="mt-3 z-[1] p-2 shadow menu menu-sm dropdown-content bg-base-100 rounded-box w-52"
            >
              <li>
                <Link to="/label" className="justify-between">
                  Label
                </Link>
              </li>

              <li className="my-2">
                <Link to="/history" className="">
                  History
                </Link>
              </li>
              <div className="flex justify-center p-2 ">
                <button onClick={logout} className="btn btn-primary w-full f">
                  Logout
                </button>
              </div>
            </ul>
          </div>
        ) : (
          <>
            <Link to="/login" />
          </>
        )}
      </div>
    </div>
  );
};

export default NavBar;
