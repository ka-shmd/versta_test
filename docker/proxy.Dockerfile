FROM node:22-alpine AS client-build
WORKDIR /app

COPY src/client/package.json src/client/package-lock.json ./
RUN npm ci

COPY src/client .
RUN npm run build

FROM caddy:2-alpine AS runtime
COPY --from=client-build /app/dist /srv
COPY docker/Caddyfile /etc/caddy/Caddyfile

EXPOSE 8080
