import { useContext, useEffect, useState } from "react";
import completion from "../openai/label.js";
import axios from "axios";
import { UserContext } from "../context/userContext.jsx";
import { toast } from "react-hot-toast";
import Confetti from "react-confetti";
import threedot from "../public/dot.png";

export default function Label() {
  const { user } = useContext(UserContext);
  const [inputValue, setInputValue] = useState("");
  const [buttonLoading, setButtonLoading] = useState(false);
  const [load, setLoading] = useState(false);
  const [procurements, setProcurements] = useState([]);
  const [business, setBusiness] = useState([]);
  const [percentages, setPercentages] = useState([]);
  const [showConfetti, setShowConfetti] = useState(false);

  useEffect(() => {
    console.log(percentages);
  }, [percentages, procurements]);
  useEffect(() => {
    getProcurements();
  }, []);

  const getProcurements = async () => {
    try {
      await axios.get("/getProcurements").then((e) => {
        setProcurements(e.data);
        console.log("Procurements,", e);
      });
    } catch (error) {
      console.log(error);
    }
  };
  const labelProcurement = async () => {
    try {
      setButtonLoading(true);
      // await getProcurements();
      const completionPromises = procurements.map((procurement) =>
        completion(procurement.description, business)
      );

      const results = await Promise.all(completionPromises);

      setPercentages(results); // Assuming completion returns an array
      setLoading(true);
      setButtonLoading(false);
      //console.log(percentages);
    } catch (error) {
      console.log(error);
    }
  };
  const addHistory = async (title, description, percentages, item) => {
    try {
      await axios
        .post("/addHistory", {
          title,
          description,
          owner: user.email,
          business_areas: business,
          percentages,
        })
        .then(() => {
          setShowConfetti(true);
          toast.success("Added to History!");
          handleDeleteItem(item);
          setInterval(() => {
            setShowConfetti(false);
          }, 5000);
        });
    } catch (error) {
      console.log(error);
    }
  };

  const handleAddItem = () => {
    setBusiness((prevArray) => [...prevArray, inputValue]);
    setInputValue("");
  };
  const handleDeleteItem = (item) => {
    setProcurements(
      procurements.filter((procurement) => procurement.title !== item.title)
    );
  };
  const handleDeleteBusiness = (item) => {
    setBusiness(business.filter((element, index) => index !== item));
  };

  return (
    <div className="flex h-screen items-center justify-between pl-36 pr-16  ">
      {showConfetti && (
        <Confetti width={window.innerWidth} height={window.innerHeight} />
      )}
      <div className="bg-slate-700 p-8 rounded shadow-md w-[40%] space-y-4 ">
        <div className="flex flex-row mb-12 ">
          <input
            type="text"
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            className="input input-bordered  w-full max-w-xs border p-2"
            placeholder="Enter the business areas..."
          />
          <button
            onClick={handleAddItem}
            className="btn  ml-6  mr-1   rounded-lg bg-gray-800 fill-none text-gray-300 p-2 py-2 px-6 text-2xl font-bold "
          >
            +
          </button>
        </div>

        <ul className="mt-12">
          {business.map((item, index) => (
            <div
              key={index}
              className="flex flex-row justify-between mr-4 text-gray-300 items-center mb-4  rounded-xl bg-gray-800 "
            >
              <li key={index} className=" items-center p-3">
                {item}
              </li>
              <button
                onClick={() => handleDeleteBusiness(index)}
                className="btn btn-square text-gray-300 curser-pointer"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-6 w-6"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth="2"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            </div>
          ))}
        </ul>
      </div>

      <div className="bg-slate-700 p-8 rounded shadow-md w-[60%] space-y-4 ml-16">
        <div className="flex flex-row mb-12 justify-center ">
          <button
            onClick={labelProcurement}
            className=" ml-6  mr-1  rounded-lg bg-primary p-3 px-6 text-xl font-normal text-gray-300 bg-gray-800 "
          >
            {buttonLoading ? (
              <span className="loading loading-spinner"></span>
            ) : (
              <span className="font-sans ">Start Labeling</span>
            )}
          </button>
        </div>

        <ul className="mt-12">
          {procurements.map((item, index) => (
            <div
              key={index}
              className="flex flex-row justify-between  items-center mr-4 mb-4  w-full h-full "
            >
              <div className="flex flex-col items-start justify-between p-4 w-full rounded-md mr-4 bg-gray-800">
                <div className="flex flex-row justify-between items-start w-full">
                  <li key={index} className="font-bold text-xl">
                    {item.title}
                  </li>
                  <button
                    className="mt-1"
                    onClick={() =>
                      document.getElementById(`my_modal_${index}`).showModal()
                    }
                  >
                    <img src={threedot} alt="" />
                  </button>
                </div>
                <dialog id={`my_modal_${index}`} className="modal">
                  <div className="modal-box w-11/12 max-w-5xl">
                    <h3 className="font-bold text-lg">{item.title}!</h3>
                    <p className="py-4">{item.description}</p>
                    <div className="modal-action">
                      <form method="dialog">
                        {/* if there is a button in form, it will close the modal */}
                        <button className="btn">Close</button>
                      </form>
                    </div>
                  </div>
                </dialog>
                <li className="font-light text-lightText">
                  {business.map((element, index2) => {
                    return (
                      <span className="font-mono" key={index2}>
                        {load &&
                          `    ${element}:${
                            percentages[index][index2] !== undefined
                              ? `${percentages[index][index2]}%`
                              : "Not labeled"
                          }`}
                      </span>
                    );
                  })}
                </li>
              </div>

              <div className="mr-8 flex flex-row ">
                <button
                  onClick={() =>
                    addHistory(
                      item.title,
                      item.description,
                      percentages[index],
                      item
                    )
                  }
                  className=" curser-pointer text-xl mr-4 w-[87px] h-[86px] hover:bg-primary	rounded-md shadow-md bg-gray-800 transition delay-130"
                >
                  👍
                </button>
                <button
                  onClick={() => handleDeleteItem(item)}
                  className="curser-pointer text-xl  w-[87px] h-[86px] hover:bg-primary rounded-md shadow-md  bg-gray-800 transition delay-130"
                >
                  👎
                </button>
              </div>
            </div>
          ))}
        </ul>
      </div>
    </div>
  );
}
