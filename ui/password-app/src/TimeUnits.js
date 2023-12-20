import React, { useState, useEffect } from 'react';
import axios from 'axios';

const TimeUnits = () => {
  const [timeUnits, setTimeUnits] = useState([]);
  const [newTimeUnitName, setNewTimeUnitName] = useState('');

  const handleAddTimeUnit = async () => {
    try {
      console.log('Adding time unit')
      await axios.post('http://localhost:5103/api/timeunits', { name: newTimeUnitName });
      setNewTimeUnitName('');
    } catch (error) {
      console.error('Error adding time unit:', error);
    }
  };

  return (
      <div>
        <h3>Add New Time Unit</h3>
        <input
          type="text"
          placeholder="Time Unit Name"
          value={newTimeUnitName}
          onChange={(e) => setNewTimeUnitName(e.target.value)}
        />
        <button onClick={handleAddTimeUnit}>Add Time Unit</button>
      </div>
  );
};

export default TimeUnits;