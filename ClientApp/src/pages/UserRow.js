import React from 'react';

const UserRow = ({ user, isSelected, onChange }) => {
    const handleChange = (event) => {
        onChange(user.id, event.target.checked);
      };
    
      return (
        <tr>
          <td>
            <input type="checkbox" checked={isSelected} onChange={handleChange} />
          </td>
          <td>{user.name}</td>
          <td>{user.email}</td>
          <td>{new Date(user.lastLoginTime).toLocaleString()}</td>
          <td>{new Date(user.registrationTime).toLocaleString()}</td>
          <td>{user.status}</td>
        </tr>
      );
};

export default UserRow;
