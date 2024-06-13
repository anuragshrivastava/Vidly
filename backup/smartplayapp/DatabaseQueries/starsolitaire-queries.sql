66ew4

select * from users;

select * from leaderboard_weekly;

select * from leaderboard_previous_weekly;

select firebase_token, google_userid,  username, profile_url, email_id, leaderboard_weekly.id, games_played, stars_won, last_updated  from users, leaderboard_weekly where users.id = leaderboard_weekly.user_id  and users.id = 1 LIMIT 1;

INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '123', 'Som1', '1', 'som@gmail.com', now()) RETURNING id;
INSERT INTO leaderboard_previous_weekly(user_id, games_played, stars_won, last_updated) VALUES (4,1,2,now());

DELETE FROM leaderboard_weekly;

INSERT INTO leaderboard_weekly(user_id, games_played, stars_won, last_updated) SELECT user_id, 0, 0, now() FROM leaderboard_previous_weekly WHERE stars_won > 0 ORDER BY stars_won DESC;

select user_id, id from leaderboard_weekly;

SELECT username, profile_url, games_played, stars_won FROM users, leaderboard_weekly WHERE leaderboard_weekly.user_id = users.id ORDER BY stars_won DESC, games_played;

SELECT username, profile_url, games_played, stars_won FROM users, leaderboard_previous_weekly WHERE leaderboard_previous_weekly.user_id = users.id ORDER BY stars_won DESC, games_played;

SELECT username, profile_url, games_played, stars_won, last_updated FROM users, leaderboard_weekly WHERE users.id = 2 AND leaderboard_weekly.user_id = users.id;

UPDATE users SET username = 'someone', profile_url = '1' WHERE id = 2;

delete from leaderboard_weekly where id = 18;

select id from leaderboard_weekly WHERE user_id = 1;

delete from users;
delete from leaderboard_weekly;
delete from leaderboard_previous_weekly;

ALTER SEQUENCE leaderboard_weekly_id_seq RESTART WITH 1


INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'hallowedTern', '2', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'unimportantBadger', '4', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'thankfulBadger', '6', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'greatBald eagle', '8', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'unimportantFish', '1', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'immenseRed panda', '0', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'miniatureMole', '5', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'punyParrot', '6', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'fitMarten', '3', '', now());
INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES ('', '', 'uglyWalrus', '1', '', now());

INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (15, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (16, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (17, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (18, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (19, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (20, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (21, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (22, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (23, true, 0, 0, now());
INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (24, true, 0, 0, now());
