create table user_tb(
U_code int,
U_id varchar(32) not null unique,
U_password varchar(32) not null,
 U_Nickname varchar(32) ,
 U_name varchar(32) not null,
 U_phone varchar(15) not null unique,
 U_email varchar(50) ,
 U_login_time datetime,
primary key(U_code)
);


create table FRIEND_TB(
  U_code int ,
   FR_code int,
  FR_name varchar(32), 
  FR_Nickname varchar(32),
  FR_phone varchar(15) unique ,
  FR_hide bool default false ,
  FR_block bool default false ,
Primary key(U_code,FR_code),
 foreign key(U_code) references USER_TB(U_code)
);

create table SERVER_TB(
S_code int,
s_name varchar(32),
creater_code int,
primary key(S_code)
);

create table server_userlist(
user_code int,
s_code int,
primary key(user_code, s_code),
foreign key (s_code) references server_TB(s_code)
);

create table s_perm_tb(
user_code int,
perm_code int,
perm_name varchar(32),
perm_001 bool DEFAULT true,
perm_002 bool DEFAULT true,
perm_003 bool DEFAULT true,
primary key(user_code, perm_code),
foreign key(user_code) references server_userlist(user_code)
);




create table ch_tb(
ch_code int,
u_code int,
s_code int,
Ch_user_name varchar(32),
ch_name varchar(32),
primary key(ch_code, u_code, s_code),
 foreign key(U_code) references USER_TB(U_code),
 foreign key(s_code) references server_TB(s_code)
);

create table msg_tb (
ch_code int,
msg_code int,
msg_text varchar(300),
msg_time time,
primary key(ch_code, msg_code),
foreign key(ch_code) references Ch_TB(ch_code)
);


create table u_cl_tb(
U_code int,
u_cl_code int,
CL_name varchar(32),
CL_content varchar(500),
CL_date date,
createer varchar(32),
primary key(u_code,u_CL_code),
foreign key(u_code) references USER_TB(u_code)
);

create table s_cl_tb(
s_code int,
s_cl_code int,
creater varchar(32),
CL_name varchar(32),
CL_content varchar(500),
CL_date date,
primary key(s_code,s_CL_code),
foreign key(s_code) references server_TB(s_code)
);

create table u_ck_tb(
u_code int,
u_CK_code int,
CK_name varchar(32),
CK_content varchar(500),
CK_date date,
CK_Check bool default false,
creater varchar(32),
CK_start_time time,
CK_end_time time,
primary key(u_CK_code,u_code),
foreign key(u_code) references user_TB(u_code)
);

create table s_ck_tb(
s_code int,
s_CK_code int,
creater varchar(32),
CK_name varchar(32),
CK_content varchar(500),
CK_date date,
CK_Check bool default false,
CK_start_time time,
CK_end_time time,
primary key(s_CK_code,s_code),
foreign key(s_code) references server_TB(s_code)
);





create table seq_table(
code int unsigned,
name varchar(32) not null);

insert into seq_table values(0,'user');
insert into seq_table values(0,'server');
insert into seq_table values(0,'perm');
insert into seq_table values(0,'msg');
insert into seq_table values(0,'u_ck');
insert into seq_table values(0,'u_cl');
insert into seq_table values(0,'s_ck');
insert into seq_table values(0,'s_cl');
insert into seq_table values(0,'friend');
insert into seq_table values(0,'ch');

    

DELIMITER $$
CREATE FUNCTION get_seq (the_name VARCHAR(32))
RETURNS int unsigned
modifies sql data
Deterministic
BEGIN
DECLARE RESULT_code int unsigned;
update seq_table set code = (code +1) where name = the_name;
select code into result_code from seq_table where name =the_name Limit 1;
return RESULT_code;
END $$

CREATE FUNCTION get_ch_seq (the_name VARCHAR(32))
RETURNS int unsigned
modifies sql data
Deterministic
BEGIN
DECLARE RESULT_code int unsigned;
update seq_table set code = (code +2) where name = the_name;
select code into result_code from seq_table where name =the_name Limit 1;
return RESULT_code;
END $$


DELIMITER $$
CREATE procedure Pro_user_in(
U_in_id varchar(32),
U_in_pw varchar(32),
U_in_Nick varchar(32),
U_in_name varchar(32),
U_in_phone varchar(15),
U_in_email varchar(50),
U_in_logintime datetime 
)
begin
insert into User_tb values((select get_seq('user')), U_in_id ,U_in_pw ,U_in_Nick ,U_in_name ,U_in_phone,U_email ,U_in_logintime);
END $$

call Pro_user_in('ID001','qwe123',null,'test1','010-9226-0219','ghrud8835@gmail.com',null)$$
call Pro_user_in('ID002','qwe123',null,'test2','010-9226-0212','ghrud0219@gmail.com',null)$$


DELIMITER $$
CREATE FUNCTION F_login (Login_id VARCHAR(32),Login_pw varchar(32))
RETURNS bool

DETERMINISTIC
BEGIN
DECLARE re_val bit;
Declare compare_pw varchar(32);
Declare compare_id varchar(32);
set re_val = false;

select u_id into compare_id from user_tb where u_id = Login_id;

if compare_id is null then
return re_val;
end if;

select U_password into compare_pw from user_tb where U_id = Login_id;
if compare_pw = login_pw then
set re_val = true;
end if;

update user_tb set U_login_time = (select sysdate()) where U_id = login_id;
return re_val;
END $$

create procedure select_u_code(p_u_id varchar(32))
 DETERMINISTIC
BEGIN
select U_code from user_Tb where U_id = P_U_id;
END $$ 

DELIMITER $$
CREATE procedure p_pr_sel (P_U_code int)

DETERMINISTIC
BEGIN
select U_id, U_nickname, U_name , U_phone,U_email from user_Tb where U_code = P_U_code;

END $$


DELIMITER $$
CREATE procedure p_set_sel (P_U_code int)
DETERMINISTIC
BEGIN
select U_id, U_nickname , U_name , U_phone,U_email  from user_tb where U_code = P_U_code;
END $$

DELIMITER $$
CREATE procedure p_set_up (P_U_code int, p_password varchar(32), p_Nick varchar(32),p_name varchar(32) , p_phone varchar(15),P_U_email varchar(50) )

DETERMINISTIC
BEGIN
update User_tb set U_password = p_password where U_code= P_U_code;
update User_tb set U_Nickname = p_Nickname where U_code = P_U_code;
update User_tb set U_name = p_name where U_code = P_U_code;
update User_tb set U_phone = p_phone where U_code = P_U_code;
update User_tb set U_email = P_U_email where U_code = P_U_code;
END $$

DELIMITER $$
CREATE procedure p_set_up_Nick (P_U_code int, p_U_nick varchar(32))
DETERMINISTIC
BEGIN
update User_tb set U_Nickname = P_U_Nick where U_code = P_U_code;
END $$
 
DELIMITER $$
CREATE procedure p_set_up_name (P_U_code int, p_U_name varchar(32))
DETERMINISTIC
BEGIN
update User_tb set U_name = P_U_name where U_code = P_U_code;
END $$
 
DELIMITER $$
CREATE procedure p_set_up_phone (P_U_code int, p_U_phone varchar(15))
DETERMINISTIC
BEGIN
update User_tb set U_phone = P_U_phone where U_code = P_U_code;
END $$



DELIMITER $$
CREATE procedure P_Fr_id_in (p_u_code int,p_fr_id varchar(32))
DETERMINISTIC
BEGIN
declare p_fr_name varchar(32);
declare p_fr_code int;
select U_name into p_fr_name from user_tb where U_id = p_fr_id;
select U_code into p_fr_code from user_tb where U_id = p_fr_id;
if p_u_code != p_fr_code then
insert into friend_tb (U_code, fr_code, fr_name) values(p_u_code,p_fr_code,p_fr_name);
end if;
END $$

DELIMITER $$
CREATE procedure P_Fr_phone_in (P_U_code int,p_fr_phone varchar(15))
DETERMINISTIC
BEGIN
declare p_fr_name varchar(32);
declare p_fr_code int;
select u_code into p_fr_code from user_tb where U_phone = p_fr_phone;
select U_name into p_fr_name from user_tb where U_phone = p_fr_phone;
if p_u_code != p_fr_code then
insert into friend_tb (U_code, fr_code, fr_name,fr_phone) 
values(p_u_code,p_fr_code,p_fr_name,p_fr_phone);
end if;

END $$


DELIMITER $$
CREATE procedure P_Fr_set_name (P_U_code int,p_fr_code int, p_set_name varchar(32))
DETERMINISTIC
BEGIN
update friend_tb set fr_name = p_set_name where U_code= p_u_code  && fr_code = p_fr_code;
END $$


DELIMITER $$
CREATE procedure P_Fr_set_nick (P_U_code int,p_fr_code int, p_set_nick varchar(32))
DETERMINISTIC
BEGIN
update friend_tb set fr_nickname = p_set_nick where U_code = p_u_code && fr_code = p_fr_code;
END $$


DELIMITER $$
CREATE procedure P_Fr_set_hide (P_U_code int,p_fr_code int,T_F bool)
DETERMINISTIC
BEGIN
update friend_tb set fr_hide = t_f where U_code = p_u_code && fr_code = p_fr_code;
END $$

DELIMITER $$
CREATE procedure P_Fr_set_block (P_U_code int,p_fr_code int,T_F bool)
DETERMINISTIC
BEGIN
update friend_tb set fr_block = t_f where U_code = p_u_code && fr_code = p_fr_code;
END $$

CREATE procedure P_Fr_set_phone (P_U_code int,p_fr_code int, p_set_phone varchar(32))
DETERMINISTIC
BEGIN
update friend_tb set fr_phone = p_set_phone where U_code= p_u_code  && fr_code = p_fr_code;
END $$


create procedure p_fr_list_select(p_u_code int)
DETERMINISTIC
BEGIN
select fr_code,fr_name, fr_nickname,fr_hide,fr_block from friend_tb where u_code = P_U_code;
END $$


create procedure p_fr_one_select (P_U_code int,p_fr_code int)
  DETERMINISTIC
  begin
select fr_code, fr_name, fr_nickname, fr_hide,fr_block from friend_tb where u_code = P_U_code && fr_code = p_fr_code ;
  end $$

create procedure p_u_ck_create(
p_u_code int,
p_ck_name varchar(32),
p_CK_content varchar(500),
p_ck_date date,
p_CK_start_time time,
p_CK_end_time time
)
DETERMINISTIC
begin
declare p_u_name varchar(32);
select u_name into p_u_name from user_Tb where U_code = p_u_code;
insert into u_ck_tb(u_code,u_ck_code, ck_name,CK_content, ck_date, CK_Check, creater,CK_start_time, CK_end_time)
  values(p_u_code,(select get_seq('u_ck')),p_ck_name, p_CK_content, 0,p_ck_date, p_u_name,p_CK_start_time, p_CK_end_time);
end $$

(select get_seq('user'))





유저 채널  u_ck_tb , u_cl_tb  두개에 대하여 만드는것
 1. 생성
 2. 수정
 3. 삭제 
 4. 테이터 읽기

서버 -  s_ck_tb , s_cl_tb 에 대해서 생성하는것 
 1. 생성
 2. 수정
 3. 삭제 
 4. 테이터 읽기 return 
서버장의 경우 s_perm_tb 를 건들여야 함 근데 
일단은 소강 상태 아직 s_perm_tb 에대해서는 일단 관리자 권한이랑
서버장에대해 아직 미정 상태임 추후 업데이트 해야함 


채널  - ch_tb 와 msg_tb 에대해 
 1. 생성
 2. 수정
 3. 삭제 
 4. 테이터 읽기











