# SpecFlow Agentic Workflow

This project implements an agentic workflow for automated test generation and execution based on OpenAPI contracts using SpecFlow. The workflow consists of three specialized agents that work together to analyze API specifications, generate test cases, and execute tests.

## Agents

### 1. OpenApiGherkinScribe

**Agent Name**: OpenApiGherkinScribe

**Purpose**: This agent reads and analyzes OpenAPI contracts to automatically generate Gherkin test cases.

**Responsibilities**:
- Parse OpenAPI specifications
- Identify API endpoints, parameters, and response types
- Generate Gherkin scenarios for positive test cases (successful operations)
- Generate Gherkin scenarios for negative test cases (error conditions)
- Generate Gherkin scenarios for edge cases (boundary values, extreme inputs)

### 2. TestCodeGenerator

**Agent Name**: TestCodeGenerator

**Purpose**: This agent reviews the Gherkin test cases and generates executable test code.

**Responsibilities**:
- Process Gherkin scenarios from OpenApiGherkinScribe
- Generate SpecFlow step definitions
- Generate acceptance tests based on the Gherkin scenarios
- Generate integration tests with concrete API client implementation
- Generate live tests for production environment testing
- Create necessary configuration files for test execution

### 3. TestExecutor

**Agent Name**: TestExecutor

**Purpose**: This agent executes the test suites and produces detailed reports.

**Responsibilities**:
- Run acceptance tests
- Run integration tests
- Run live tests
- Capture test execution logs
- Generate test summary reports
- Generate detailed test reports
- Generate test coverage reports

## Workflow Process

1. **OpenAPI Analysis**:
   - The workflow begins with an OpenAPI contract as input
   - OpenApiGherkinScribe parses the contract and generates Gherkin scenarios

2. **Test Code Generation**:
   - TestCodeGenerator takes the Gherkin scenarios and produces executable test code
   - Generates appropriate step definitions and test implementations

3. **Test Execution**:
   - TestExecutor runs the test suites across different environments
   - Collects test results and generates comprehensive reports

## Project Structure

```
AgenticSpecFlowWorkflow/
├── src/
│   ├── Agents/
│   │   ├── OpenApiGherkinScribe.cs
│   │   ├── TestCodeGenerator.cs
│   │   └── TestExecutor.cs
│   ├── Common/
│   │   ├── ApiEndpoint.cs
│   │   ├── AgentContext.cs
│   │   ├── AgentResult.cs
│   │   ├── GherkinScenario.cs
│   │   ├── IAgent.cs
│   │   ├── OpenApiParser.cs
│   │   ├── ReportGenerator.cs
│   │   ├── TestCodeFactory.cs
│   │   └── TestRunner.cs
│   ├── Workflow/
│   │   └── WorkflowOrchestrator.cs
│   ├── AgenticSpecFlowWorkflow.csproj
│   └── Program.cs
├── SampleOpenAPI.json
└── AgenticSpecFlowWorkflow.sln
```

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or VS Code with C# extensions

### Build and Run

```bash
# Clone the repository
git clone <repository-url>
cd AgenticSpecFlowWorkflow

# Build the project
dotnet build

# Run the workflow
dotnet run --project src/AgenticSpecFlowWorkflow.csproj -- /path/to/your/openapi.json
```

### Configuration

The workflow can be configured through command-line arguments or by modifying the `WorkflowConfig` in `Program.cs`.

## Output

The workflow generates the following output structure:

```
WorkflowOutput/
├── Stage1_GherkinScenarios/
│   └── Features/
│       ├── get.feature
│       ├── post.feature
│       └── ...
├── Stage2_TestImplementations/
│   ├── AcceptanceTests/
│   │   ├── GivenSteps.cs
│   │   ├── WhenSteps.cs
│   │   ├── ThenSteps.cs
│   │   └── TestContext.cs
│   ├── IntegrationTests/
│   │   ├── ApiClient.cs
│   │   ├── Models/
│   │   └── *Tests.cs
│   └── LiveTests/
│       └── *LiveTests.cs
└── Stage3_TestReports/
    ├── test-summary-report.html
    ├── test-detailed-report.html
    └── test-coverage-report.html
```

## Extension Points

This workflow can be extended in several ways:

1. Add more agent types for specialized tasks
2. Enhance OpenAPI parsing capabilities
3. Add support for more test frameworks
4. Implement real test execution against APIs
5. Add CI/CD integration

## License

This project is licensed under the MIT License - see the LICENSE file for details.