Feature: API Testing
  As an API consumer
  I want to test the API endpoints
  So that I can ensure they work correctly

Scenario: Get user by ID
  Given the API is available
  When I request a user with ID 123
  Then I should get a successful response
  And the response should contain user information