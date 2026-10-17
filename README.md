# ThreeDeeEeeCentralAuthService

A small authentication and authorization API for my portfolio platform. It handles user login, registration, JWT access tokens, refresh tokens, and client app validation. The MyPortfolio MVC app calls this service for all auth‑related operations.

Runs in Docker and uses PostgreSQL for storing users, tokens, and client records. Built with a clean layered structure (Domain, Application, Infrastructure, API) for clarity and maintainability.

## Overview
ThreeDeeEeeCentralAuthService provides a focused Web API for handling user authentication, token issuing, and refresh token management. It is designed to run inside Docker and uses PostgreSQL for persistence.

This service also exposes endpoints consumed directly by the MyPortfolio MVC application, allowing the frontend to authenticate users, request tokens, and validate sessions through a clean, centralized API.

## Features
- User registration and login
- JWT access token generation
- Refresh token issuing and rotation
- PostgreSQL-backed user and token storage
- Endpoints used by the MyPortfolio MVC frontend
- Clean layered architecture (Domain, Application, Infrastructure, API)

## Architecture

The project follows a structured layout:
- Domain – core entities and rules
- Application – use cases and interfaces
- Infrastructure – EF Core, repositories, configuration
- Api – controllers and endpoints

## Deployment
The service runs as a container inside a multi‑container setup. Secrets and environment‑specific values are provided through environment variables.

## License
MIT License
