권한 테이블 생성 및 설정들 

create procedure p_userlist_create(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into serveruserlist (user_code , s_code) values(p_u_code, p_s_code);
end $$

create procedure p_userlist_s(
p_s_code int
)
DETERMINISTIC
begin
select user_code from server_userlist where s_code = p_s_code;
end $$

create procedure p_userlist_u(
p_u_code int
)
DETERMINISTIC
begin
select s_code from server_userlist where u_code = p_u_code;
end $$


create procedure p_userlist_u(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
delete from server_userlist where s_code = p_s_code && u_code = p_u_code;
end $$


create procedure p_perm_create_all(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into s_perm_tb(user_code, s_code,perm_code, perm_name, perm_001, peem_002,perm_003)
values(p_u_code,s_code,(select get_seq('perm')),'all',1,1,1);
end $$

create procedure p_perm_create_read(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into s_perm_tb(user_code, s_code,perm_code, perm_name, perm_001, peem_002,perm_003)
values(p_u_code,s_code,(select get_seq('perm')),'read',1,0,0);
end $$

create procedure p_perm_create_readanddrop(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into s_perm_tb(user_code, s_code,perm_code, perm_name, perm_001, peem_002,perm_003)
values(p_u_code,s_code,(select get_seq('perm')),'read_drop',1,0,1);
end $$

create procedure p_perm_create_readwrite(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into s_perm_tb(user_code, s_code,perm_code, perm_name, perm_001, peem_002,perm_003)
values(p_u_code,s_code,(select get_seq('perm')),'read_write',1,1,0);
end $$

create procedure p_perm_create_write(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into s_perm_tb(user_code, s_code,perm_code, perm_name, perm_001, peem_002,perm_003)
values(p_u_code,s_code,(select get_seq('perm')),'write',0,1,0);
end $$

create procedure p_perm_create_writedrop(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into s_perm_tb(user_code, s_code,perm_code, perm_name, perm_001, peem_002,perm_003)
values(p_u_code,s_code,(select get_seq('perm')),'writedrop',0,1,1);
end $$

create procedure p_perm_create_drop(
p_u_code int,
p_s_code int
)
DETERMINISTIC
begin
insert into s_perm_tb(user_code, s_code,perm_code, perm_name, perm_001, peem_002,perm_003)
values(p_u_code,s_code,(select get_seq('perm')),'drop',0,0,1);
end $$




