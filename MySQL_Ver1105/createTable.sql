# create table

# create user table
create table user_tb(
U_code int,
U_id varchar(32) not null unique,
U_pwd varchar(32) not null,
U_nick varchar(32),
U_name varchar(32) not null,
U_phone varchar(15) not null unique,
U_email varchar(50),
U_content varchar(500),
U_recent_time datetime,
is_deleted bool default false,
primary key(U_code)
);

# create friend table
create table friend_tb(
u_code int,
fr_code int,
fr_nick varchar(32),
fr_hide bool default false,
fr_block bool default false,
is_deleted bool default false,
primary key(u_code, fr_code),
foreign key(u_code) references user_tb(U_code),
foreign key(fr_code) references user_tb(U_code)
);

# create user channel table
# state : 1-message, 2-checklist, 3-calendar, 4-project
create table u_channel_tb(
u_code int,
ch_code int,
ch_title varchar(50),
state int,
is_private bool default false,
is_deleted bool,
primary key(u_code, ch_code),
foreign key(u_code) references user_tb(U_code)
);

# create user calendar table
create table u_calendar_tb(
u_code int,
ch_code int,
cal_code int,
content varchar(500),
start_time BIGINT,
end_time BIGINT,
is_private bool default false,
is_deleted bool,
primary key(u_code, ch_code, cal_code),
foreign key(u_code, ch_code) references u_channel_tb(u_code,ch_code)
);

# create user checklist table
create table u_checklist_tb(
u_code int,
ch_code int,
chk_code int,
content varchar(500),
start_time BIGINT,
end_time BIGINT,
checked bool default false,
is_private bool default false,
is_deleted bool,
primary key(u_code, ch_code, chk_code),
foreign key(u_code, ch_code) references u_channel_tb(u_code, ch_code)
);

# create server table
create table server_tb(
S_code int,
S_title varchar(50),
S_admin_code int,
is_deleted bool,
primary key(S_code),
foreign key(S_admin_code) references user_tb(U_code)
);

# create server user table
create table server_user_tb(
s_code int,
u_code int,
cal_is_private bool default false,
is_single bool,
is_deleted bool,
primary key(s_code, u_code),
foreign key(s_code) references server_tb(S_code),
foreign key(u_code) references user_tb(U_code)
);

# create chaneel table
create table channel_tb(
s_code int,
ch_code int,
ch_title varchar(50),
state int,
is_deleted bool,
primary key(s_code, ch_code),
foreign key(s_code) references server_tb(S_code)
);

# create checklist table
create table checklist_tb(
s_code int,
ch_code int,
chk_code int,
content varchar(500),
creater int,
start_time BIGINT,
end_time BIGINT,
checked bool default false,
is_private bool default false,
is_deleted bool,
primary key(s_code, ch_code, chk_code),
foreign key(s_code, ch_code) references channel_tb(s_code, ch_code),
foreign key(creater) references user_tb(U_code)
);

# create message table
create table msg_tb(
s_code int,
ch_code int,
msg_code int,
content varchar(500),
creater int,
start_time BIGINT,
is_private bool default false,
is_deleted bool,
primary key(s_code, ch_code, msg_code),
foreign key(s_code, ch_code) references channel_tb(s_code, ch_code),
foreign key(creater) references user_tb(U_code)
);

# create calendar
create table calendar_tb(
s_code int,
ch_code int,
cal_code int,
content varchar(500),
creater int,
start_time BIGINT,
end_time BIGINT,
is_private bool default false,
is_deleted bool,
primary key(s_code, ch_code, cal_code),
foreign key(s_code, ch_code) references channel_tb(s_code, ch_code),
foreign key(creater) references user_tb(U_code)
);

# sequence table
create table seq_table(
seq_code int unsigned,
seq_name varchar(32) not null);

# sequence table setting
insert into seq_table values(0,'user');
insert into seq_table values(0,'friend');
insert into seq_table values(0,'server');
insert into seq_table values(0,'ch');
insert into seq_table values(0,'u_chk');
insert into seq_table values(0,'u_cal');
insert into seq_table values(0,'msg');
insert into seq_table values(0,'chk');
insert into seq_table values(0,'cal');

