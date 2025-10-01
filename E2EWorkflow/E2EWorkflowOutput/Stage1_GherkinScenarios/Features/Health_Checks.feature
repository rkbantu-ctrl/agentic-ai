Feature: Health Checks
  Part of Attachment Service API v1.0.8
  As an API consumer
  I want to test the API endpoints
  So that I can ensure they work correctly

Scenario: Health Check - Verify API is available
  Given the API base URL is configured
  When I send a health check request
  Then I should receive a 200 OK response
  And the response should contain health status information

