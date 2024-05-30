import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './PasswordList.css';

const PasswordList = () => {
  const [passwords, setPasswords] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);

  const [passwordForm, setPasswordForm] = useState({
    passwordId: '',
    passwordText: '',
    strength: '',
    categoryName: '',
    timeUnitName: '',
    value: '',
    offlineCrackSec: '',
    rankAlt: '',
    fontSize: '',
  });

  const fetchData = async () => {
    try {
      const response = await axios.get(`http://localhost:5103/api/passwords/with-relations?page=${currentPage}`);
      console.log(response.data);
      setPasswords(response.data);
    } catch (error) {
      console.error('Error fetching passwords:', error);
    }
  };

  useEffect(() => {
    fetchData();
  }, [currentPage]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setPasswordForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleAddOrUpdatePassword = async () => {
    try {
       // Input validation - We want to validate basic input as close to the view as possible.
    if (
      !passwordForm.passwordText ||
      !passwordForm.strength ||
      !passwordForm.fontSize ||
      !passwordForm.value ||
      !passwordForm.offlineCrackSec ||
      !passwordForm.rankAlt ||
      !passwordForm.categoryName ||
      !passwordForm.timeUnitName
    ) {
      alert('All fields are required.');
      return;
    }

    if (passwordForm.passwordId > 0) {
      // Send a PUT request to update the password
      await axios.put(`http://localhost:5103/api/passwords/${passwordForm.passwordId}`, {
        password_value: passwordForm.passwordText,
        category_name: passwordForm.categoryName,
        time_unit_name: passwordForm.timeUnitName,
        strength: passwordForm.strength.toString(),
        font_size: passwordForm.fontSize.toString(),
        value: passwordForm.value.toString(),
        offline_crack_sec: passwordForm.offlineCrackSec.toString(),
        rank_alt: passwordForm.rankAlt.toString(),
      });
    } else {
      // Send a POST request to add a new password
      await axios.post('http://localhost:5103/api/passwords/add', {
        password_value: passwordForm.passwordText,
        category_name: passwordForm.categoryName,
        time_unit_name: passwordForm.timeUnitName,
        strength: passwordForm.strength,
        font_size: passwordForm.fontSize,
        value: passwordForm.value,
        offline_crack_sec: passwordForm.offlineCrackSec,
        rank_alt: passwordForm.rankAlt,
      });
    }

      // After adding/updating, refetch data to update the list
      fetchData();
      // Clear the form
      setPasswordForm({
        passwordId: '',
        passwordText: '',
        strength: '',
        categoryName: '',
        timeUnitName: '',
        value: '',
        offlineCrackSec: '',
        rankAlt: '',
        fontSize: '',
      });
    } catch (error) {
      console.error('Error adding/updating password:', error);
    }
  };

  const handleEditPassword = (password) => {
    // Set the values in the form for editing

    setPasswordForm({
      passwordId: password.passwordId,
      passwordText: password.passwordText,
      strength: password.strength,
      categoryName: password.categoryName,
      timeUnitName: password.timeUnitName,
      value: password.value,
      offlineCrackSec: password.offlineCrackSec,
      rankAlt: password.rankAlt,
      fontSize: password.fontSize,
    });
  };

  const handleDeletePassword = async (passwordId) => {
    try {
      // Send a DELETE request to remove the password
      await axios.delete(`http://localhost:5103/api/passwords/${passwordId}`);
      // After deleting, refetch data to update the list
      fetchData();
    } catch (error) {
      console.error('Error deleting password:', error);
    }
  };

  return (
    <div className="container">
      <div className="new-password-form">
        <h3>{passwordForm.passwordId > 0 ? 'Update' : 'Add New'} Password</h3>
        <label>
          Password Text:
          <input type="text" name="passwordText" value={passwordForm.passwordText} onChange={handleInputChange} />
        </label>
        <label>
          Strength:
          <input type="number" name="strength" value={passwordForm.strength} onChange={handleInputChange} />
        </label>
        <label>
          Category:
          <input type="text" name="categoryName" value={passwordForm.categoryName} onChange={handleInputChange} />
        </label>
        <label>
          Time Unit:
          <input type="text" name="timeUnitName" value={passwordForm.timeUnitName} onChange={handleInputChange} />
        </label>
        <label>
          Value:
          <input type="number" name="value" value={passwordForm.value} onChange={handleInputChange} />
        </label>
        <label>
          Offline Crack Sec:
          <input type="number" name="offlineCrackSec" value={passwordForm.offlineCrackSec} onChange={handleInputChange} />
        </label>
        <label>
          Rank Alt:
          <input type="number" name="rankAlt" value={passwordForm.rankAlt} onChange={handleInputChange} />
        </label>
        <label>
          Font Size:
          <input type="number" name="fontSize" value={passwordForm.fontSize} onChange={handleInputChange} />
        </label>
        <button onClick={handleAddOrUpdatePassword}>
          {passwordForm.passwordId > 0 ? 'Update Password' : 'Add Password'}
        </button>
      </div>

      {/* Password List with Relations */}
      <h2>Password List with Relations</h2>
      <ul className="password-list">
        {passwords.map((password) => (
          <li key={password.passwordId} className="password-item">
            <strong>Password Id:</strong> {password.passwordId}<br />
            <strong>Password Text:</strong> {password.passwordText}<br />
            <strong>Strength:</strong> {password.strength}<br />
            <strong>Category:</strong> {password.categoryName}<br />
            <strong>Time Unit:</strong> {password.timeUnitName}<br />
            <strong>Value:</strong> {password.value}<br />
            <strong>Offline Crack Sec:</strong> {password.offlineCrackSec}<br />
            <strong>Rank Alt:</strong> {password.rankAlt}<br />
            <strong>Font Size:</strong> {password.fontSize}<br />
            <button onClick={() => handleEditPassword(password)}>Edit Password</button>
            <button onClick={() => handleDeletePassword(password.passwordId)}>Delete Password</button>
          </li>
        ))}
      </ul>

      <div className="pagination">
        <button onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}>Previous Page</button>
        <span> Page {currentPage} </span>
        <button onClick={() => setCurrentPage((prev) => prev + 1)}>Next Page</button>
      </div>
    </div>
  );
};

export default PasswordList;
