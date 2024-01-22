import React from "react";

const ProcurementCard = ({
  title,
  description,
  percentages,
  business_areas,
  index,
}) => {
  return (
    <>
      <tr>
        <th>{index}</th>
        <td>{title}</td>
        <td>{description}</td>
        <td>
          {business_areas.map((element, index) => {
            return (
              <span
                key={index}
              >{`${element}:  ${percentages[index]}%      `}</span>
            );
          })}
        </td>
      </tr>
    </>
  );
};

export default ProcurementCard;
