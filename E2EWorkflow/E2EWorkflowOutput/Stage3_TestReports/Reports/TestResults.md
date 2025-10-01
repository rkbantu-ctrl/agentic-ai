# Attachment Service API Test Results

Generated: 09/30/2025 20:11:04

## Summary

- **Total Tests:** 17
- **Passed:** 12
- **Failed:** 5
- **Success Rate:** 70.6%

## Test Results

| Scenario | Category | Status | Execution Time (ms) |
|----------|----------|--------|---------------------|
| Health Check - Verify API is available | Health Checks | ❌ Failed | 510 |
| Upload a file with metadata | Files | ✅ Passed | 1321 |
| POST /api/v1/files - Error handling | Files | ✅ Passed | 952 |
| Download a file | Files | ✅ Passed | 1534 |
| GET /api/v1/files/{attachmentId} - Error handling | Files | ❌ Failed | 1299 |
| Delete a file | Files | ✅ Passed | 1905 |
| DELETE /api/v1/files/{attachmentId} - Error handling | Files | ✅ Passed | 1533 |
| Update file metadata | Files | ❌ Failed | 161 |
| PATCH /api/v1/files/{attachmentId} - Error handling | Files | ✅ Passed | 821 |
| List of files by key | Files | ✅ Passed | 1197 |
| GET /api/v1/files/{key}/{keyValue} - Error handling | Files | ❌ Failed | 593 |
| Retrieve file metadata | Files | ✅ Passed | 845 |
| GET /api/v1/files/{attachmentId}/metadata - Error handling | Files | ❌ Failed | 863 |
| Copy a file to a new location with a new name | Copy-file | ✅ Passed | 854 |
| POST /copy-file/{attachmentId}/{newFileName} - Error handling | Copy-file | ✅ Passed | 1241 |
| Health Check | Healthcheck | ✅ Passed | 1861 |
| GET /healthcheck - Error handling | Healthcheck | ✅ Passed | 1097 |

## Failed Tests Details

### Health Check - Verify API is available

**Error:** JSON parsing error: Unexpected token

```
   at TestProject.StepDefinitions.Health_ChecksSteps.And_the_response_should_contain_health_status_information() in Health_ChecksSteps.cs:line 87
```

### GET /api/v1/files/{attachmentId} - Error handling

**Error:** Expected value 'active' but found 'inactive'

```
   at TestProject.StepDefinitions.FilesSteps.Then_I_should_receive_a_401_Unauthorized_response() in FilesSteps.cs:line 64
```

### Update file metadata

**Error:** Expected value 'active' but found 'inactive'

```
   at TestProject.StepDefinitions.FilesSteps.And_the_response_should_have_the_correct_content_type() in FilesSteps.cs:line 61
```

### GET /api/v1/files/{key}/{keyValue} - Error handling

**Error:** Connection refused

```
   at TestProject.StepDefinitions.FilesSteps.Then_I_should_receive_a_401_Unauthorized_response() in FilesSteps.cs:line 50
```

### GET /api/v1/files/{attachmentId}/metadata - Error handling

**Error:** Server returned internal error (500)

```
   at TestProject.StepDefinitions.FilesSteps.Then_I_should_receive_a_401_Unauthorized_response() in FilesSteps.cs:line 55
```

