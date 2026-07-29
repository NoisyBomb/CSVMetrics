# CSVMetrics

Web API для загрузки, обработки и анализа временных рядов из CSV-файлов. Приложение принимает CSV с результатами измерений, валидирует и сохраняет данные в PostgreSQL, автоматически рассчитывая интегральные показатели по каждому файлу.

## Стек технологий

- **.NET 10**
- **ASP.NET Core Web API** (контроллеры)
- **Entity Framework Core** + **Npgsql** (PostgreSQL)
- **FluentValidation**
- **CsvHelper**
- **Swagger / OpenAPI**
- **xUnit** — юнит- и интеграционные тесты

## Архитектура

Проект построен по принципу слоистой архитектуры с чётким направлением зависимостей:

```
CSVMetrics.Api            → контроллеры, middleware, конфигурация
CSVMetrics.Application    → бизнес-логика: сервисы, валидация, DTO, интерфейсы репозиториев
CSVMetrics.Infrastructure → EF Core, репозитории, миграции
CSVMetrics.Domain         → доменные сущности
```

Domain не зависит ни от чего. Application зависит только от Domain. Infrastructure реализует интерфейсы, объявленные в Application. Api зависит от Application и Infrastructure.

## Структура решения

```
src/
 ├─ CSVMetrics.Domain          — сущности MeasurementValue, FileResult
 ├─ CSVMetrics.Application     — CsvParser, CsvValidator, AggregateCalculator,
 │                               CsvUploadService, ResultsQueryService,
 │                               RecentValuesQueryService, интерфейсы репозиториев
 ├─ CSVMetrics.Infrastructure  — AppDb (DbContext), конфигурации маппинга,
 │                               репозитории, миграции
 └─ CSVMetrics.API             — MeasurementsController, ExceptionHandler,
                                  Program.cs

tests/
 ├─ CSVMetrics.UnitTests         — тесты валидатора и калькулятора агрегатов
 └─ CSVMetrics.IntegrationTests  — end-to-end тест метода загрузки файла
```

## Модель данных

### Values

Хранит каждую строку из загруженных CSV-файлов.

| Поле | Тип | Описание |
|---|---|---|
| Id | long | Первичный ключ |
| FileName | string | Имя исходного файла |
| Date | datetimeoffset | Время начала операции |
| ExecutionTime | double | Время выполнения, сек |
| Value | double | Значение показателя |

Индекс: составной `(FileName, Date)` — используется методом получения последних значений по файлу.

### Results

Хранит интегральные показатели по каждому загруженному файлу.

| Поле | Тип | Описание |
|---|---|---|
| Id | long | Первичный ключ |
| FileName | string | Имя файла (уникальное) |
| TimeDeltaSeconds | double | max(Date) − min(Date), сек |
| StartDate | datetimeoffset | Момент запуска первой операции |
| AvgExecutionTime | double | Среднее время выполнения |
| AvgValue | double | Среднее значение показателя |
| MedianValue | double | Медиана значений показателя |
| MaxValue | double | Максимальное значение показателя |
| MinValue | double | Минимальное значение показателя |
| ProcessedAt | datetimeoffset | Момент обработки файла |

Индексы: уникальный на `FileName` (обеспечивает upsert-логику при повторной загрузке файла с тем же именем), плюс индексы на `StartDate`, `AvgValue`, `AvgExecutionTime` — под фильтрацию.

## API

### 1. Загрузка CSV-файла

```
POST /api/measurements/upload
Content-Type: multipart/form-data
```

Параметр: `file` — CSV-файл в формате:

```
Date;ExecutionTime;Value
2024-01-15T10:30:00.0000Z;1.5;42.3
2024-01-15T11:00:00.0000Z;2.0;38.7
```

**Валидация:**
- Дата не может быть позже текущего момента и не может быть раньше `2000-01-01`
- `ExecutionTime` не может быть отрицательным
- `Value` не может быть отрицательным
- Количество строк: от 1 до 10 000
- Все поля обязательны и должны соответствовать своим типам

Если хотя бы одно условие нарушено — файл считается невалидным, изменения в БД не сохраняются (валидация выполняется до открытия транзакции), клиенту возвращается `400 Bad Request` со списком ошибок.

Если файл с таким именем уже был загружен ранее — старые данные (строки в `Values` и агрегаты в `Results`) удаляются, новые данные записываются взамен, в рамках одной транзакции.

**Успешный ответ** (`200 OK`) — рассчитанные агрегаты:

```json
{
  "id": 1,
  "fileName": "test.csv",
  "timeDeltaSeconds": 1800,
  "startDate": "2024-01-15T10:30:00+00:00",
  "avgExecutionTime": 1.75,
  "avgValue": 40.5,
  "medianValue": 40.5,
  "maxValue": 42.3,
  "minValue": 38.7,
  "processedAt": "2026-07-28T12:00:00+00:00"
}
```

### 2. Получение списка результатов с фильтрами

```
GET /api/measurements/results
```

Query-параметры (все опциональны, могут комбинироваться):

| Параметр | Тип | Описание |
|---|---|---|
| FileName | string | Точное имя файла |
| StartDateFrom / StartDateTo | datetimeoffset | Диапазон времени запуска первой операции |
| AvgValueFrom / AvgValueTo | double | Диапазон среднего значения показателя |
| AvgExecutionTimeFrom / AvgExecutionTimeTo | double | Диапазон среднего времени выполнения |

Возвращает список объектов `Results`, соответствующих переданным фильтрам.

### 3. Последние значения по файлу

```
GET /api/measurements/{fileName}/recent
```

Возвращает последние 10 записей из `Values` для указанного файла, отсортированные по `Date` в порядке убывания (от самой свежей к самой старой).

## Обработка ошибок

Все необработанные исключения перехватываются глобальным middleware (`ExceptionHandler`) и возвращаются клиенту в формате `application/problem+json` (RFC 7807), без утечки внутренних деталей реализации. Полная информация об ошибке логируется на сервере.

## Тесты

- **Юнит-тесты** (`CSVMetrics.UnitTests`): правила валидации CSV-строк (диапазон дат, неотрицательность значений), расчёт агрегатов (медиана для чётного/нечётного количества значений, граничный случай одной строки, расчёт дельты времени).
- **Интеграционный тест** (`CSVMetrics.IntegrationTests`): полный цикл загрузки CSV через `WebApplicationFactory` — от HTTP-запроса до сохранения в реальной базе данных и проверки рассчитанных агрегатов в ответе.

