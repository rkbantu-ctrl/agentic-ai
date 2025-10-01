Feature: Healthcheck
  Part of Attachment Service API v1.0.8
  As an API consumer
  I want to test the API endpoints
  So that I can ensure they work correctly

Scenario: Health Check
  Given the API base URL is configured
  And I am authenticated with a valid token
  When I send a GET request to "/healthcheck"
  Then I should receive a successful response
  And the response should have the correct content type
  And the response should contain the requested data

Scenario: GET /healthcheck - Error handling
  Given the API base URL is configured
  When I send a request with an invalid resource ID
  Then I should receive a 404 Not Found response
  Given I have an invalid authentication token
  When I send a GET request to "/healthcheck"
  Then I should receive a 401 Unauthorized response

