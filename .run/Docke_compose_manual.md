# staging
docker build -f "app.api/Dockerfile_Staging" --progress=plain -t "oo.api.app:0.1.1-stag" . 
or
docker compose up -d app.api.staging app.sql.staging

# production
docker build -f "app.api/Dockerfile_Production" --progress=plain -t "oo.api.app:0.1" .
or
docker compose up -d "app.api.production"

# down
docker compose down
