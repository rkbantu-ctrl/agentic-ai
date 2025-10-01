Feature: Copy-file
  Part of Attachment Service API v1.0.8
  As an API consumer
  I want to test the API endpoints
  So that I can ensure they work correctly

Scenario: Copy a file to a new location with a new name
  Given the API base URL is configured
  And I am authenticated with a valid token
  And I have valid request data
  When I send a POST request to "/copy-file/{attachmentId}/{newFileName}"
  Then I should receive a successful response
  And the response should have the correct content type
  And the response should include the created resource
  And the resource should be persisted

Scenario: POST /copy-file/{attachmentId}/{newFileName} - Error handling
  Given the API base URL is configured
  And I have invalid request data
  When I send a POST request to "/copy-file/{attachmentId}/{newFileName}"
  Then I should receive a 400 Bad Request response
  And the response should include validation errors
  Given I have an invalid authentication token
  When I send a POST request to "/copy-file/{attachmentId}/{newFileName}"
  Then I should receive a 401 Unauthorized response

