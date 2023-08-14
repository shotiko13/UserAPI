import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { Container, Form, Button, Alert, Row, Col } from 'react-bootstrap';
import { AiOutlineUser, AiOutlineMail, AiOutlineLock } from 'react-icons/ai';

const LoginPage = () => {
    const [emailOrUsername, setEmailOrUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState(null);
    const navigate = useNavigate();
    const handleSubmit = async (e) => {
        e.preventDefault();
        const url = 'https://hw4Shota.somee/api/Users/login';
        try {
          const response = await axios.post(url, {
              emailOrUsername: emailOrUsername,
              password: password
          });
          localStorage.setItem("token", response.data.token);
          navigate('/userManagement');
      } catch (error) {
          if (error.response) {
              if (error.response.status === 403) {
                  setError('Your account is blocked');
              } else if (error.response.status === 400) {
                  setError('Username or password is incorrect');
              }
          } else {
              setError('An error occured during login.')
          }
      }
      
    }; 

    return (
        <Container>
          {error && <Alert variant="danger">{error}</Alert>}
    
          <Form onSubmit={handleSubmit}>
            <Row className="justify-content-center">
              <Col xs={12} sm={10} md={8} lg={6}>
                <Form.Group controlId="emailOrUsername">
                  <Form.Label><AiOutlineMail /> Email or Username</Form.Label>
                  <Form.Control type="text" value={emailOrUsername} onChange={e => setEmailOrUsername(e.target.value)} />
                </Form.Group>
    
                <Form.Group controlId="password">
                  <Form.Label><AiOutlineLock /> Password</Form.Label>
                  <Form.Control type="password" value={password} onChange={e => setPassword(e.target.value)} />
                </Form.Group>
    
                <Button variant="primary" type="submit">Login</Button>
              </Col>
            </Row>
          </Form>
        </Container>
      );
}

export default LoginPage;