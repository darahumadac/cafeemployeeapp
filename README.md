### how to run
```bash
# Development
# run the backend 
dotnet run -e Development
# run the frontend
npm run dev


#run using Docker
docker compose up --build -d

```

#### Hosts
- App/UI Host: `http://localhost:5173`  to view the app

- API Host: `http://localhost:5191`
    - openapi works only in dev env - `http://localhost:5191/openapi/v1.json`
