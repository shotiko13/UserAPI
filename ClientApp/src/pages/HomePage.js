import React from 'react';
import { Link } from 'react-router-dom';
import { Container, Button, Row, Col } from 'react-bootstrap';
import { PersonFill, BoxArrowInRight } from 'react-bootstrap-icons';

const HomePage = () => {
  return (
    <Container fluid className="d-flex align-items-center justify-content-center vh-100">
      <Row>
        <Col className="text-center">
          <h1 className="mb-4">Hello, welcome to Task 4!</h1>
          <p className="mb-4">  
            My name is Shota Vashakmadze. This is a project to manage users.
          </p>
          <p className="mb-4">
            If you're already registered, please proceed to the login page.
          </p>
          <Button variant="primary" as={Link} to="/login" className="mb-3">
            <BoxArrowInRight className="mb-1"/> Login
          </Button>
          <p className="mb-4">
            If you're not yet registered, please proceed to the registration page.
          </p>
          <Button variant="primary" as={Link} to="/register" className="mb-3">
            <PersonFill className="mb-1"/> Register
          </Button>
        </Col>
      </Row>
    </Container>
  );
}

export default HomePage;
