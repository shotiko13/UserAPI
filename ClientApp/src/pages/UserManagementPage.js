import React, { useEffect, useState } from 'react';
import UserRow from './UserRow';
import Toolbar from './Toolbar';
import { Table, Container } from 'react-bootstrap';
import axios from 'axios';

const UserManagementPage = () => {
    const [users, setUsers] = useState([]);
    const [selectedUsers, setSelectedUsers] = useState(new Set());

    const fetchUsers = async () => {
        try {
            const response = await axios.get('https://localhost:5057/api/Users');
            if (response.status === 200) {
                setUsers(response.data);
            } else {
                const errorMessage = await response.text();
                console.error(errorMessage);
            }
        } catch (error) {
            console.error('An error occurred while fetching the users:', error);         
        }
    };

    useEffect(() => {
        fetchUsers();
    }, []);
      
    const handleUserChange = (userId, isSelected) => {
        console.log('Before modification, selectedUsers:', [...selectedUsers]);
        console.log(`Changing selection state for user ${userId} to ${isSelected}`);
        const newSelectedUsers = new Set(selectedUsers);
        if (isSelected) {
            newSelectedUsers.add(userId);
        } else {
            newSelectedUsers.delete(userId);
        }
        console.log('After modification, newSelectedUsers:', [...newSelectedUsers]);
        setSelectedUsers(new Set(newSelectedUsers));
    }
    

    const handleSelectAll = () => {
        const allUserIds = users.map(user => user.id);
        setSelectedUsers(allUserIds);
    }

    const handleDeselectAll = () => {
        setSelectedUsers(new Set());
    }

    const handleStatus = async (response) => {
        if (response.status === 200) 
            fetchUsers();
        else {
            const errorMessage = await response.text();
            console.error(errorMessage);
        }
    } 

    const handleBlock = async (userId) => {
        try {
            const response = await axios.post(`https://localhost:5057/api/Users/block/${userId}`);
            handleStatus(response);
        } catch (error) {
            console.error('An error occurred while blocking the user');
        }
    }

    const handleUnblock = async (userId) => {
        try {
            const response = await axios.post(`https://localhost:5057/api/Users/unblock/${userId}`)
            handleStatus(response);
        } catch (error) {
            console.error('An error occured while unblocking the user');
        }
    }

    const handleDelete = async (userId) => {
        try {
            const response = await axios.post(`https://localhost:5057/api/Users/delete/${userId}`)
            handleStatus(response);
        } catch (error) {
            console.error('An error occured while deleting the user');
        }
    }

    const handleBlockSelected = () => {
        selectedUsers.forEach(user => {
            handleBlock(user.id);
        });
    }
    
    const handleUnblockSelected = () => {
        selectedUsers.forEach(user => {
            handleUnblock(user.id);
        });
    }
    
    const handleDeleteSelected = () => {
        selectedUsers.forEach(user => {
            handleDelete(user.id);
        });
    }

    return (
        <Container className="mt-4">
          <Toolbar
            onSelectAll={handleSelectAll}
            onDeselectAll={handleDeselectAll}
            onBlock={handleBlockSelected}
            onUnblock={handleUnblockSelected}
            onDelete={handleDeleteSelected}
          />
          <Table striped bordered hover className="mt-3">
            <thead>
              <tr>
                <th>#</th>
                <th>Name</th>
                <th>Email</th>
                <th>Last Login Time</th>
                <th>Registration Time</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {users.map(user => (
                <UserRow
                  key={user.id}
                  user={user}
                  isSelected={selectedUsers.has(user.id)}
                  onChange={handleUserChange}
                />
              ))}
            </tbody>
          </Table>
        </Container>
      );
    };
    
export default UserManagementPage;
    