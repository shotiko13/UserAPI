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
            const response = await axios.get('https://hw4Shota.somee/api/Users');
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
        const newSelectedUsers = new Set(selectedUsers);
        if (isSelected) 
            newSelectedUsers.add(userId);
        else 
            newSelectedUsers.delete(userId);
        setSelectedUsers(new Set(newSelectedUsers));
    }
    

    const handleSelectAll = () => {
        const allUserIds = new Set(users.map(user => user.id));
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

    const handleOperation = async (userId, operation) => {
        try {
            const response = await axios.post(`https://hw4Shota.somee/api/Users/${operation}/${userId}`);
            handleStatus(response);
        } catch (error) {
            console.error(`An error occured while ${operation}ing the user`);
        }
    }

    const handleBlock = async (userId) => {
        handleOperation(userId, 'block');
    }

    const handleUnblock = async (userId) => {
        handleOperation(userId, 'unblock');
    }

    const handleDelete = async (userId) => {
        handleOperation(userId, 'delete');
    }

    const handleBlockSelected = () => {
        selectedUsers.forEach(userId => {
            handleBlock(userId);
        });
    }
    
    const handleUnblockSelected = () => {
        selectedUsers.forEach(userId => {
            handleUnblock(userId);
        });
    }
    
    const handleDeleteSelected = () => {
        selectedUsers.forEach(userId => {
            handleDelete(userId);
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
    