-- Script para deletar todos os dados das tabelas de jogos, playoffs e estatísticas.
-- Ordem respeitando FKs (tabelas que referenciam outras vêm primeiro).
-- Executar na pasta Server: sqlite3 Basketball.db < delete_stats_and_games.sql

-- Desativa checagem de FK para evitar erro na ordem (opcional no SQLite)
PRAGMA foreign_keys = OFF;

-- Tabelas que referenciam Game ou Playoffs (apagar primeiro)
DELETE FROM GameNews;
DELETE FROM PlayoffGames;
DELETE FROM PlayerGameStats;
DELETE FROM TeamGameStats;   -- referencia Games; necessário antes de apagar Games
DELETE FROM PlayerPlayoffsStats;
DELETE FROM PlayerRegularStats;
DELETE FROM TeamPlayoffsStats;
DELETE FROM TeamRegularStats;

-- Tabelas principais
DELETE FROM Games;
DELETE FROM Playoffs;

-- Reativa checagem de FK
PRAGMA foreign_keys = ON;

-- Opcional: zerar sequência de Id (SQLite: sqlite_sequence)
DELETE FROM sqlite_sequence WHERE name IN (
  'GameNews', 'Games', 'PlayerGameStats', 'PlayerPlayoffsStats',
  'PlayerRegularStats', 'PlayoffGames', 'Playoffs', 'TeamGameStats',
  'TeamPlayoffsStats', 'TeamRegularStats'
);

SELECT 'Limpeza concluída.' AS resultado;
