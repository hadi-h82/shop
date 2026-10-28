FROM node:22-alpine AS build

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production


FROM node:22-alpine AS final

WORKDIR /app

ENV NODE_ENV=production
ENV PORT=8000

COPY package.json package-lock.json ./
RUN npm ci --omit=dev && npm cache clean --force

COPY --from=build /app/dist/shop-client ./dist/shop-client

EXPOSE 8000

CMD ["node", "dist/shop-client/server/server.mjs"]