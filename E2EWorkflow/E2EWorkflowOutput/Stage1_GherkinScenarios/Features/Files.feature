Feature: Files
  Part of Attachment Service API v1.0.8
  As an API consumer
  I want to test the API endpoints
  So that I can ensure they work correctly

Scenario: Upload a file with metadata
  Given the API base URL is configured
  And I am authenticated with a valid token
  And I have valid request data
  When I send a POST request to "/api/v1/files"
  Then I should receive a successful response
  And the response should have the correct content type
  And the response should include the created resource
  And the resource should be persisted

Scenario: POST /api/v1/files - Error handling
  Given the API base URL is configured
  And I have invalid request data
  When I send a POST request to "/api/v1/files"
  Then I should receive a 400 Bad Request response
  And the response should include validation errors
  Given I have an invalid authentication token
  When I send a POST request to "/api/v1/files"
  Then I should receive a 401 Unauthorized response

Scenario: Download a file
  Given the API base URL is configured
  And I am authenticated with a valid token
  When I send a GET request to "/api/v1/files/{attachmentId}"
  Then I should receive a successful response
  And the response should have the correct content type
  And the response should contain the requested data

Scenario: GET /api/v1/files/{attachmentId} - Error handling
  Given the API base URL is configured
  When I send a request with an invalid resource ID
  Then I should receive a 404 Not Found response
  Given I have an invalid authentication token
  When I send a GET request to "/api/v1/files/{attachmentId}"
  Then I should receive a 401 Unauthorized response

Scenario: Delete a file
  Given the API base URL is configured
  And I am authenticated with a valid token
  When I send a DELETE request to "/api/v1/files/{attachmentId}"
  Then I should receive a successful response
  And the response should have the correct content type

Scenario: DELETE /api/v1/files/{attachmentId} - Error handling
  Given the API base URL is configured
  When I send a request with an invalid resource ID
  Then I should receive a 404 Not Found response
  Given I have an invalid authentication token
  When I send a DELETE request to "/api/v1/files/{attachmentId}"
  Then I should receive a 401 Unauthorized response

Scenario: Update file metadata
  Given the API base URL is configured
  And I am authenticated with a valid token
  And I have valid partial update data
  When I send a PATCH request to "/api/v1/files/{attachmentId}"
  Then I should receive a successful response
  And the response should have the correct content type

Scenario: PATCH /api/v1/files/{attachmentId} - Error handling
  Given the API base URL is configured
  When I send a request with an invalid resource ID
  Then I should receive a 404 Not Found response
  Given I have an invalid authentication token
  When I send a PATCH request to "/api/v1/files/{attachmentId}"
  Then I should receive a 401 Unauthorized response

Scenario: List of files by key
  Given the API base URL is configured
  And I am authenticated with a valid token
  When I send a GET request to "/api/v1/files/{key}/{keyValue}"
  Then I should receive a successful response
  And the response should have the correct content type
  And the response should contain the requested data

Scenario: GET /api/v1/files/{key}/{keyValue} - Error handling
  Given the API base URL is configured
  When I send a request with an invalid resource ID
  Then I should receive a 404 Not Found response
  Given I have an invalid authentication token
  When I send a GET request to "/api/v1/files/{key}/{keyValue}"
  Then I should receive a 401 Unauthorized response

Scenario: Retrieve file metadata
  Given the API base URL is configured
  And I am authenticated with a valid token
  When I send a GET request to "/api/v1/files/{attachmentId}/metadata"
  Then I should receive a successful response
  And the response should have the correct content type
  And the response should contain the requested data

Scenario: GET /api/v1/files/{attachmentId}/metadata - Error handling
  Given the API base URL is configured
  When I send a request with an invalid resource ID
  Then I should receive a 404 Not Found response
  Given I have an invalid authentication token
  When I send a GET request to "/api/v1/files/{attachmentId}/metadata"
  Then I should receive a 401 Unauthorized response

