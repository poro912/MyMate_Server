# Test Data

# user
call db_server.create_user('admin', '1234', 'admin', 'admin', '010-0000-0000', 'admin@gmail.com');
call db_server.create_user('psy', '1234', 'sungyun', 'sungyun', '010-1111-1111', 'psy@gamil.com');
call db_server.create_user('poro', '1234', 'muhyeon', 'muhyeon', '010-2222-2222', 'poro@gmail.com');
call db_server.create_user('genie', '1234', 'jin', 'jin', '010-3333-3333', 'genie@gmail.com');

select * from user_tb;

# friend
call db_server.create_friend(2, 'poro', 'null');
call db_server.create_friend(2, 'genie', 'null');
call db_server.create_friend(3, 'genie', 'null');

select * from friend_tb;

# userChannel
call db_server.create_u_channel(2, 'messageChannel', 1, false);
call db_server.create_u_channel(2, 'checklistChannel', 2, false);
call db_server.create_u_channel(2, 'calendarChannel', 3, false);
call db_server.create_u_channel(2, 'projectChannel', 4, false);
call db_server.create_u_channel(3, 'messageChannel', 1, false);
call db_server.create_u_channel(3, 'checklistChannel', 2, false);
call db_server.create_u_channel(3, 'calendarChannel', 3, false);
call db_server.create_u_channel(3, 'projectChannel', 4, false);
call db_server.create_u_channel(4, 'messageChannel', 1, false);
call db_server.create_u_channel(4, 'checklistChannel', 2, false);
call db_server.create_u_channel(4, 'calendarChannel', 3, false);
call db_server.create_u_channel(4, 'projectChannel', 4, false);

select * from u_channel_tb;

# userChecklist
call db_server.create_u_checklist(2, 4, 'sungyun\'sTask1', 20211114, 20211114, false);
call db_server.create_u_checklist(3, 12, 'poro\'sTask1', 20211114, 20211114, false);
call db_server.create_u_checklist(4, 20, 'genie\'sTask1', 20211114, 20211114, false);

select * from u_checklist_tb;

# userCalendar
call db_server.create_u_calendar(2, 6, 'sungyun\'sCalendar1', 20221114, 20221114, false);
call db_server.create_u_calendar(3, 14, 'poro\'sCalendar1', 20221114, 20221114, false);
call db_server.create_u_calendar(4, 22, 'genie\'sCalendar1', 20221114, 20221114, false);

select * from u_calendar_tb;

# server
call db_server.create_server('ycsStudyRoom', 3, false);
call db_server.create_server('ycsTackRoom', 2, false);
call db_server.create_server('2-6TaskRoom', 3, false);

select * from server_tb;

# serverUser
call db_server.create_server_user(1, 2);
call db_server.create_server_user(1, 3);
call db_server.create_server_user(1, 4);
call db_server.create_server_user(2, 2);
call db_server.create_server_user(2, 3);
call db_server.create_server_user(2, 4);
call db_server.create_server_user(3, 2);
call db_server.create_server_user(3, 3);
call db_server.create_server_user(3, 4);

select * from server_user_tb;

# channel
call db_server.create_channel(1, 'smallTalk', 1);
call db_server.create_channel(1, 'studyList', 2);
call db_server.create_channel(1, 'studyCalendar', 3);
call db_server.create_channel(2, 'smallTalk', 1);
call db_server.create_channel(2, 'taskList', 2);
call db_server.create_channel(2, 'taskCalendar', 3);
call db_server.create_channel(3, 'smallTalk', 1);
call db_server.create_channel(3, '2-6TaskList', 2);
call db_server.create_channel(3, '2-6TaskCalendar', 3);

select * from channel_tb;

# message
call db_server.create_message(1, 32, 'helloworld', 2, 20221114, false);
call db_server.create_message(2, 38, 'helloworld', 3, 20221114, false);
call db_server.create_message(3, 44, 'helloworld', 3, 20221114, false);

select * from msg_tb;

# checklist
call db_server.create_checklist(1, 34, 'studyTask1', 2, 20221114, 20221114, false);
call db_server.create_checklist(2, 40, 'studyTask1', 3, 20221114, 20221114, false);
call db_server.create_checklist(3, 46, 'studyTask1', 3, 20221114, 20221114, false);


select * from checklist_tb;

# calendar
call db_server.create_calendar(1, 36, 'studyCalendar1', 2, 20221114, 20221114, false);
call db_server.create_calendar(2, 42, 'studyCalendar1', 3, 20221114, 20221114, false);
call db_server.create_calendar(3, 48, 'studyCalendar1', 3, 20221114, 20221114, false);


select * from calendar_tb;
