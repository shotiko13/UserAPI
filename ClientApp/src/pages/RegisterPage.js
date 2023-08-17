import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { Container, Form, Button, Alert, Row, Col } from 'react-bootstrap';
import { AiOutlineUser, AiOutlineMail, AiOutlineLock } from 'react-icons/ai';

const RegisterPage = () => {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    setError(null); 

    const url = 'https://hw4Shota.somee.com/api/Users/register';

    try {
      await axios.post(url, {
        name: username,
        emailAddress: email,
        password: password,
        confirmPassword: confirmPassword,
      });

      navigate('/');
    } catch (error) {
      setError('An error occurred during registration. Please try again.');
      console.error('An error occurred during registration:', error);
    }
  };

  return (
    <Container>
      {error && <Alert variant="danger">{error}</Alert>}

      <Form onSubmit={handleSubmit}>
        <Row className="justify-content-center">
          <Col xs={12} sm={10} md={8} lg={6}>
            <Form.Group controlId="username">
              <Form.Label><AiOutlineUser /> Username</Form.Label>
              <Form.Control type="text" value={username} onChange={e => setUsername(e.target.value)} />
            </Form.Group>

            <Form.Group controlId="email">
              <Form.Label><AiOutlineMail /> Email</Form.Label>
              <Form.Control type="email" value={email} onChange={e => setEmail(e.target.value)} />
            </Form.Group>

            <Form.Group controlId="password">
              <Form.Label><AiOutlineLock /> Password</Form.Label>
              <Form.Control type="password" value={password} onChange={e => setPassword(e.target.value)} />
            </Form.Group>

            <Form.Group controlId="confirmPassword">
              <Form.Label><AiOutlineLock /> Confirm Password</Form.Label>
              <Form.Control type="password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} />
            </Form.Group>

            <Button variant="primary" type="submit">Register</Button>
          </Col>
        </Row>
      </Form>
    </Container>
  );
};

export default RegisterPage;
