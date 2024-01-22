import React, { useState, useEffect, useContext } from "react";
import axios from "axios";
import { UserContext } from "../context/userContext";
import ProcurementCard from "../components/ProcurementCard";
function History() {
  const { user, email } = useContext(UserContext);
  const [procurements, setProcurements] = useState([]);

  useEffect(() => {
    const getProcurements = async () => {
      try {
        const procurement = await axios.post("/getLabeledProcurements", {
          owner: email,
        });
        setProcurements(procurement.data);
      } catch (error) {
        console.log(error);
      }
    };

    if (email) {
      getProcurements();
    }
  }, [email]);

  useEffect(() => {}, [procurements]);

  return (
    <div className="overflow-x-auto">
      <table className="table table-zebra">
        {/* head */}
        <thead>
          <tr>
            <th></th>
            <th>Title</th>
            <th>Description</th>
            <th>Business areas</th>
          </tr>
        </thead>
        <tbody>
          {procurements.map((procurement, index) => {
            return (
              <ProcurementCard
                title={procurement.title}
                description={procurement.description}
                percentages={procurement.percentages}
                business_areas={procurement.business_areas}
                index={index}
              />
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

export default History;
