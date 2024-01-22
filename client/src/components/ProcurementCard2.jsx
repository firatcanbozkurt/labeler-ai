import React from "react";

const ProcurementCard = ({ title, description, lastDayToApply }) => {
  return (
    //tailwindcss
    // <div className="flex flex-col justify-center items-center">
    //     <div className="max-w-xs rounded overflow-hidden shadow-lg my-2">
    //         <img
    //             className="w-full"
    //             src="https://tailwindcss.com/img/card-top.jpg"
    //             alt="Sunset in the mountains"
    //         />
    //         <div className="px-6 py-4">
    //             <div className="font-bold text-xl mb-2">{title}</div>
    //             <p className="text-gray-700 text-base">{description}</p>
    //         </div>
    //         <div className="px-6 pt-4 pb-2">
    //             <span className="inline-block bg-gray-200 rounded-full px-3 py-1 text-sm font-semibold text-gray-700 mr-2">
    //                 #photography
    //             </span>
    //             <span className="inline-block bg-gray-200 rounded-full px-3 py-1 text-sm font-semibold text-gray-700 mr-2">
    //                 #travel
    //             </span>
    //             <span className="inline-block bg-gray-200 rounded-full px-3 py-1 text-sm font-semibold text-gray-700">
    //                 #winter
    //             </span>
    //         </div>
    //     </div>
    // </div>

    <div className="procurement-card">
      <h2>{title}</h2>
      <p>{description}</p>
      <p>Last day to apply: {lastDayToApply}</p>
    </div>
  );
};

export default ProcurementCard;
