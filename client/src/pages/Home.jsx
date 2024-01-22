import React, { useState, useEffect } from "react";
import { motion, useAnimation } from "framer-motion";
import home1 from "../public/home1.jpg";
import home2 from "../public/home2.jpg";
import home3 from "../public/home3.jpg";
import aibrain from "../public/aibrain.jpg";
import aichip from "../public/aichip.jpg";
import aiman from "../public/aiman.jpg";
import aiwomen from "../public/illuwomen.jpg";
import { useNavigate } from "react-router-dom";

const images = [home1, home2, aibrain];

const imageVariants = {
  hidden: { opacity: 0 },
  visible: { opacity: 1 },
};

const textVariants = {
  hidden: { x: -200, opacity: 0 },
  visible: { x: 0, opacity: 1, transition: { duration: 1.0 } },
};
const transition = {
  duration: 0.5,
  ease: "easeInOut",
};

function Home() {
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const navigate = useNavigate();
  const controls = useAnimation();

  useEffect(() => {
    const intervalId = setInterval(() => {
      setCurrentImageIndex((prevIndex) => (prevIndex + 1) % images.length);
    }, 3000);

    return () => clearInterval(intervalId);
  }, [currentImageIndex]);

  useEffect(() => {
    controls.start("visible");
  }, [controls]);

  return (
    <>
      <motion.div
        initial="hidden"
        animate="visible"
        variants={imageVariants}
        transition={transition}
        style={{
          position: "relative",
          overflow: "hidden",
          width: "100%",
          height: "100%",
        }}
      >
        <hr className="my-2 border-t border-white" />
        <div className="h-[750px]">
          <div className="flex w-1/3 p-8 ml-32 mt-12">
            <motion.h2
              className=" text-white text-6xl   p-8 mt-4 font-bold leading-tight"
              variants={textVariants}
              initial="hidden"
              animate={controls}
            >
              Automate your procurement labeling process with Labeler-AI
            </motion.h2>
          </div>
          <div className="flex flex-col w-1/3 p-6 ml-24 mt-12">
            <motion.p
              className="font-semibold"
              variants={textVariants}
              initial="hidden"
              animate={controls}
            >
              Simplify the process of labeling public procurements with our
              AI-powered service
            </motion.p>
            <motion.button
              className="btn glass mt-6"
              onClick={() => navigate("/register")}
            >
              Get Started
            </motion.button>
          </div>
        </div>
        <div className="">
          {images.map((image, index) => (
            <motion.img
              key={index}
              src={image}
              alt={`Home Page Image ${index + 1}`}
              className="rounded-xl"
              style={{
                position: "absolute",
                top: 130,
                right: 90,
                width: "50%",
                height: "80%",
                zIndex: 0,
              }}
              initial="hidden"
              animate={index === currentImageIndex ? "visible" : "hidden"}
              variants={imageVariants}
              transition={transition}
            />
          ))}
        </div>
      </motion.div>
      <div className="w-full h-full flex mt-64 ">
        <div className="ml-24 w-1/2">
          <img src={aiwomen} alt="" className="w-3/4 h-5/5 rounded-2xl " />
        </div>
        <div className="flex flex-col justify-center items-center p-8 mr-24  w-1/2">
          <motion.h2 className="capitalize text-4xl font-bold text-white flex justify-center  mb-10">
            Automate labeling tasks
          </motion.h2>
          <motion.p className="text-xl">
            With Labeler AI, you can automate the labeling tasks for public
            procurements, allowing you to focus on other important aspects of
            your business.
          </motion.p>
        </div>
      </div>
      <div className="w-full h-full flex mt-36">
        <div className="flex flex-col justify-center items-center p-8 ml-24  w-1/2">
          <motion.h2 className="capitalize text-4xl font-bold text-white flex justify-center items-center mb-10">
            Increase accuracy and efficiency
          </motion.h2>
          <motion.p className="text-xl">
            Labeler AI's AI-powered technology ensures accurate and efficient
            labeling of public procurements, reducing erros and improving
            overall productivity.
          </motion.p>
        </div>
        <div className="ml-24 w-1/2 ">
          <img src={aichip} alt="" className="w-3/4 h-5/5 rounded-2xl" />
        </div>
      </div>
    </>
  );
}

export default Home;
