# Логика работы сервера "Морской бой"

## Общий процесс работы сервера

1. **Аутентификация и регистрация:**
   - Пользователи регистрируются с уникальными именами и паролями.
   - После успешной регистрации или входа сервер выдает токен (JWT), который используется для аутентификации последующих запросов.

2. **Создание игры:**
   - Игрок может создать новую игру, указав противника или выбрав опцию игры с ботом.
   - Сервер инициализирует игровое состояние, включая доски игроков и параметры игры.

3. **Ход игры:**
   - Игроки совершают ходы поочередно, передавая координаты.
   - Сервер проверяет валидность хода, обновляет состояние доски и сообщает результат хода всем участникам.

4. **Завершение игры:**
   - Игра завершается, когда все корабли одного из игроков потоплены.
   - Сервер фиксирует результат игры, обновляет статистику и уведомляет участников.

5. **Лидерборд и статистика:**
   - Игроки могут просматривать таблицу лидеров и свою статистику.

---

## Основная логика работы сервера

### 1. Аутентификация
**Контроллер:** `AuthController`

Методы:
- `Register`: Создает нового пользователя.
- `Login`: Проверяет учетные данные и выдает токен.

Логика:
- Проверка уникальности имени пользователя при регистрации.
- Хеширование пароля для безопасного хранения.

Пример кода для регистрации:
```csharp
public async Task<AuthResult> RegisterAsync(RegisterRequest request)
{
    if (await _userRepository.ExistsAsync(request.Username))
        return new AuthResult { Success = false, Errors = new[] { "Username already exists" } };

    var user = new User
    {
        Username = request.Username,
        PasswordHash = _passwordHasher.HashPassword(request.Password)
    };

    await _userRepository.AddAsync(user);

    var token = _jwtService.GenerateToken(user);

    return new AuthResult { Success = true, Token = token, Username = user.Username };
}
```

---

### 2. Создание и управление играми
**Контроллер:** `GamesController`

Методы:
- `CreateGame`: Создает новую игру.
- `GetGame`: Возвращает текущее состояние игры.
- `MakeMove`: Обрабатывает ход игрока.

Логика:
- При создании игры инициализируются доски игроков (10x10 клеток) с расставленными кораблями.
- Ходы обрабатываются последовательно, с проверкой на очередность и валидность.

Пример создания игры:
```csharp
public async Task<GameResult> CreateGameAsync(CreateGameRequest request)
{
    var game = new Game
    {
        Id = Guid.NewGuid().ToString(),
        PlayerOneId = request.CreatorId,
        PlayerTwoId = request.OpponentId,
        BoardPlayerOne = InitializeBoard(),
        BoardPlayerTwo = InitializeBoard(),
        Status = GameStatus.Waiting,
        Turn = request.CreatorId
    };

    await _gameRepository.AddAsync(game);

    return new GameResult { GameId = game.Id, Status = game.Status };
}
```

---

### 3. Логика хода
**Сервис:** `GamesService`

Метод: `MakeMove`
- Проверяет, чья очередь ходить.
- Проверяет, была ли клетка уже атакована.
- Обновляет состояние клетки ("попадание", "промах", "потоплен").
- Проверяет, завершена ли игра.

Пример обработки хода:
```csharp
public async Task<MoveResult> MakeMoveAsync(string gameId, MoveRequest request)
{
    var game = await _gameRepository.GetGameAsync(gameId);
    if (game.Turn != request.PlayerId)
        return new MoveResult { Success = false, Errors = new[] { "Not your turn!" } };

    var board = game.Turn == game.PlayerOneId ? game.BoardPlayerTwo : game.BoardPlayerOne;
    var cell = board.First(c => c.X == request.X && c.Y == request.Y);

    if (cell.State != CellState.Empty)
        return new MoveResult { Success = false, Errors = new[] { "Invalid move!" } };

    if (cell.HasShip)
    {
        cell.State = CellState.Hit;
        if (IsShipSunk(board, cell))
            CheckGameOver(game);
    }
    else
    {
        cell.State = CellState.Miss;
    }

    game.Turn = game.Turn == game.PlayerOneId ? game.PlayerTwoId : game.PlayerOneId;
    await _gameRepository.UpdateGameAsync(game);

    return new MoveResult { Success = true, UpdatedBoard = board };
}
```

---

### 4. Уведомления через SignalR
**Хаб:** `GameHub`

- Отправляет уведомления о начале игры, смене хода и завершении игры.

Пример уведомления о смене хода:
```csharp
await _hubContext.Clients.Group(gameId).SendAsync("TurnChanged", new
{
    GameId = gameId,
    CurrentPlayerId = nextPlayerId
});
```

---

### Пример игры
1. **Игрок A создает игру**, указав противника B.
   - Сервер создает новую игру с уникальным ID.
   - Оба игрока уведомляются о создании игры через SignalR.

2. **Игрок A делает ход**, атакуя клетку (3, 5).
   - Сервер проверяет валидность хода.
   - Если в клетке есть корабль, состояние меняется на "попадание".
   - Сервер уведомляет обоих игроков о результате хода.

3. **Игрок B делает ход**, атакуя клетку (1, 1).
   - Сервер проверяет, завершена ли игра.
   - Если все корабли одного из игроков потоплены, игра завершается.

4. **Завершение игры**:
   - Сервер фиксирует победителя.
   - Обновляет статистику игроков.
   - Отправляет уведомление о завершении игры.

---

Если потребуется углубиться в любую часть логики или добавить детали, дай знать!

