FROM nginx:alpine

# Copy the index.html
COPY index.html /usr/share/nginx/html/index.html

# Create Nginx configuration to serve SPA
RUN echo 'server { \
    listen 80; \
    server_name _; \
    root /usr/share/nginx/html; \
    index index.html; \
    \
    # Cache busting for assets \
    location ~* \.(js|css|png|jpg|jpeg|gif|ico)$ { \
        expires 1y; \
        add_header Cache-Control "public, immutable"; \
    } \
    \
    # Main app - serve index.html for all routes \
    location / { \
        try_files $uri $uri/ /index.html; \
        add_header Cache-Control "no-cache, no-store, must-revalidate"; \
    } \
}' > /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]