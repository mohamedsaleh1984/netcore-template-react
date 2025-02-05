#### DotnetCore WebAPI with React App in Typescript
This is a template for Dotnetcore WebAPI ready with JWT Auth, Cors, NSwag.

Run Backend
```
dotnet build && dotnet run
```
install node packages
```
npm install
```
React App in TypeScript
```
npm start
```
Generate Frontend Client API in Typescript
** Make sure the backend is running on designated port in package.json
```
npm run client
```

#### Generate JWT Key
``` 
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
```
