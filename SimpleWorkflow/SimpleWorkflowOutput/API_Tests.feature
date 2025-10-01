Feature: Attachment Service API Testing
  The Attachment Service provides an API for managing file attachments with metadata, 
utilizing OAuth 2.0 security. It enables users to upload medical attachments as 
evidence for insurance procedures. 

Each submission undergoes Optical Character Recognition (OCR) processing to extract 
textual information, which is subsequently analyzed using AI technology. The AI system 
evaluates the information against established Care Guidelines to ensure compliance and 
support insurance claims verification. 

This streamlined process enhances the accuracy and efficiency of handling medical 
attachment uploads.

  As an API consumer
  I want to test the API endpoints
  So that I can ensure they work correctly

Scenario: Verify API Health Check
  Given the API is available
  When I send a health check request
  Then I should receive a successful response

# This API has 6 endpoints that would be tested
# In a full implementation, each endpoint would have multiple test scenarios
