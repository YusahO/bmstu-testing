```
sudo tshark -i 3 -Y "tcp.port == 9999 and http.request" 
Running as user "root" and group "root". This could be dangerous.
Capturing on 'any'
 1856 54.896234192          ::1 → ::1          HTTP 761 GET / HTTP/1.1 
 1975 60.816873083          ::1 → ::1          HTTP 768 GET /swagger HTTP/1.1 
 1978 60.819736551          ::1 → ::1          HTTP 779 GET /swagger/index.html HTTP/1.1 
 1982 60.857235207          ::1 → ::1          HTTP 670 GET /swagger/swagger-ui.css HTTP/1.1 
 1983 60.857311187          ::1 → ::1          HTTP 662 GET /swagger/swagger-ui-bundle.js HTTP/1.1 
 1988 60.857676708          ::1 → ::1          HTTP 673 GET /swagger/swagger-ui-standalone-preset.js HTTP/1.1 
 2091 61.318716571          ::1 → ::1          HTTP 670 GET /swagger/v1/swagger.json HTTP/1.1 
 2094 61.331270856          ::1 → ::1          HTTP 719 GET /swagger/favicon-32x32.png HTTP/1.1 
 2819 91.748906015          ::1 → ::1          HTTP/JSON 827 POST /api/Auth/Registration HTTP/1.1 , JSON (application/json)
 4328 162.054102733          ::1 → ::1          HTTP 1250 GET /api/Users/dae720da-8725-4b4f-a62c-96998563ef72/playlists HTTP/1.1 
```