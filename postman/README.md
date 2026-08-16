# CricArena Postman collection

Import both JSON files into Postman:

1. `CricArena.postman_collection.json`
2. `CricArena.local.postman_environment.json`

Select **CricArena Local** as the active environment. The collection uses `{{baseUrl}}`, which is set to `https://localhost:7019`.

## Recommended order

1. Run **Auth → Register** once for a new test account.
2. Run **Auth → Login**. Its test script stores the JWT in `{{token}}`.
3. Run the Player requests as needed.
4. Run **Clubs → Create Club**. Its test script stores the response ID in `{{clubId}}`.
5. Set `{{memberPlayerId}}` manually before testing member role changes.

All protected requests inherit bearer authentication from the collection. Requests that require elevated permissions are labelled `(Admin)`.

## Convention for future controllers

Add a new top-level folder named after the controller, then add one request per action using:

- `{{baseUrl}}/api/{Controller}/{route}`
- collection-level bearer authentication for protected endpoints
- `Content-Type: application/json` for JSON bodies
- request names that describe the action and permission, such as `Update Club (Admin)`
- test scripts that store returned IDs in collection variables when later requests depend on them
