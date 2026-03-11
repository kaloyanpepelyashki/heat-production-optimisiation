# Semester Project - Group 3

## Danfoss Heat Distribution System Optimization

---

## Overview

Our project delivers a microservice-based software solution designed to optimize **Danfoss's existing district heat distribution system**. The system targets two primary objectives:

- **Cost Efficiency** - Reducing the operational costs of heat distribution by scheduling and managing heating output based on data about average energy prices.
- **Electricity Sales Profitability** - Maximizing revenue from electricity generated as a byproduct of heat production by identifying optimal market windows when electricity spot prices are at their highest.

By integration of energy market data, our microservices layer on top of Danfoss's current infrastructure to optimize ensuring the right action is taken at the right time for each respective goal.

---

## Key Features

| Feature | Description |
|---|---|
| **Heat Production Scheduling** | Schedules heat production during periods of low energy cost, reducing operational expenditure. |
| **Electricity Price Monitoring** | Continuous monitoring of electricity spot market prices to identify optimal windows for selling surplus electricity. |
| **Profitability Optimization** | Automatically triggers electricity sales when market prices exceed defined profitability thresholds. |
| **Microservice Architecture** | Each feature is implemented as an independent, loosely coupled microservice, ensuring scalability and maintainability. |

---

## Current Progress

### Project Setup & Planning
- **Jira configured for SCRUM** - Tasks have been broadly described and distributed across separate sprints following the SCRUM framework.
- **Agile development paradigm** adopted as the guiding methodology for the project lifecycle.
- **Group contract agreement** signed by all members, establishing shared responsibilities and expectations.
- **Distinctive roles assigned** to group members to ensure clear ownership of tasks, responsibilities and accountability.

### Infrastructure & DevOps
- **Monorepo architecture decided** - All services and components will be co-located in a single repository to streamline dependency management and deployment.
- **CI/CD pipeline setup in progress** - Working on establishing a Continuous Integration / Continuous Deployment (CI/CD) DevOps workflow to automate testing and deployment across services.

---

## Tech Stack

- **Backend:** The system is developed in **C# using the .NET platform**, with the application architecture split into **independent microservices** corresponding to the different system modules (e.g., Asset Manager, Source Data Manager, Optimizer, etc.).  
- **Containerization:** All services are packaged using **Docker**, ensuring consistent environments across development, testing, and deployment.  
- **CI/CD:** A **CI/CD pipeline** is used to automate **unit testing, building, and deployment**, improving reliability and enabling continuous integration of new features.  
- **Deployment:** Services are deployed and exposed through **Render**, providing a managed hosting environment for running the containerized microservices.  
- **Database:** The project uses **Supabase** as a **Database-as-a-Service (DBaaS)** solution. Data is stored in a **relational PostgreSQL database**, enabling structured data storage and efficient querying across the system components.

---

## Team

- Natanael Fejes - Group Leader
- Kaloyan Pepelyashki - SCRUM Master
- Samuel Toman - Product Owner
- Matous Hlobil
- Maksim Vasilev
- David Takac

---

## License

- Not decided
