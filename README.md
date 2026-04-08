```powershell
docker run -d `
  --name cqrs-postgres `
  -e POSTGRES_DB=cqrs_intro `
  -e POSTGRES_USER=postgres `
  -e POSTGRES_PASSWORD=postgres `
  -p 5432:5432 `
  -v pgdata_cqrs:/var/lib/postgresql/data `
  postgres:16
```