# ClientCardApi_Example
Данное кроссплатформенное api предназначено в качестве демонстрации работы с несколькими сторонними сервисами.
В данном проекте реализована работа с:
- Postgres SQL (база данных);
- PgAdmin (администрирование баз данных);
- Elastic Search (поисковая система);
- Kibana (программная панель визуализации данных);
- Prometheus (программное приложение, используемое для мониторинга событий и оповещения);
- Grafana (программная система визуализации данных);
- Consul (платформа для обнаружения сервисов и сервисная сетка для сегментации сервисов в любом облаке или среде выполнения, а также распределенное хранилище ключей и значений для конфигурации приложений.)

Вся инфраструктура запускается с помощью docker-compose.

После разворачивания "инфраструктуры" в docker следует провести миграцию баз данных для установки таблиц и всего необходимого посредством "AuthMigrations" и "Migrations" apphost приложений.
После успешных миграций авторизированный пользователь может взаимодействовать с базой данных через "контроллеры" с методами: POST (add), GET (read), PUT (update), DELETE (delete). Заполнив форму ввиде json представления.

"Main page"
![alt tag](https://github.com/AlexanderMeshchaninov/Screenshots/blob/main/Main%20screen.png "Main page")

Неавторизированные пользователи имеют доступ лишь к Health check где с помощью простого вызова можно получить ответ "Alive" и код ответа 200 или сообщение о том, что сервис "упал".

"Health check"
![alt tag](https://github.com/AlexanderMeshchaninov/Screenshots/blob/main/HealthCheck.png "Health check")

Система поиска находится в самом ClientCardApi
"Elastic search"
![alt tag](https://github.com/AlexanderMeshchaninov/Screenshots/blob/main/Services.png "Elastic search")

В проект также добавлены:
 - Логирование Serilog запись в консоль и в файл, директория LOGS;
 - Mapper;
 - Microsoft.AspNetCore.Identity.EntityFrameworkCore;
 - Fluent validator;
 - Jwt Bearer;
 - Короткие юнит-тесты;
 - Docker-compose file.
