u_cl_tb 에 대하여 생수삭읽


create procedure p_u_cl_create(
p_u_code int,
p_cL_name varchar(32),
p_CL_content varchar(500),
p_cL_date date
)
DETERMINISTIC
begin
declare p_u_name varchar(32);
select u_name into p_u_name from user_Tb where U_code = p_u_code;
insert into u_cl_tb (u_code, u_cl_code, cl_name, CL_content,cl_date,creater)
	values(p_u_code, (select get_seq('u_cl')),p_cl_name,p_CL_content,p_cL_date,p_u_name);
end $$

create procedure p_u_cl_select(
p_u_code int,
p_u_cl_code int
)
DETERMINISTIC
begin
select cl_name, CL_content, cl_date, creater from u_cl_tb where U_code = p_u_code && u_cl_code = p_u_cl_code;
end $$


create procedure p_u_cl_update_name(
p_u_code int,
p_u_ck_code int,
p_cl_name varchar(32)
)
DETERMINISTIC
begin
update u_cl_tb set cl_name = p_cL_name where where U_code = p_u_code && u_cl_code = p_u_cl_code;
end $$

create procedure p_u_cl_update_content(
p_u_code int,
p_u_ck_code int,
p_Cl_content varchar(500)
)
DETERMINISTIC
begin
update u_cl_tb set Cl_content = p_Cl_content where where U_code = p_u_code && u_cl_code = p_u_cl_code;
end $$

create procedure p_u_cl_update_date(
p_u_code int,
p_u_ck_code int,
p_Cl_date date
)
DETERMINISTIC
begin
update u_ck_tb set Cl_date = p_Cl_date where where U_code = p_u_code && u_cl_code = p_u_cl_code;
end $$


create procedure p_u_cl_delete(
p_u_code int,
p_u_ck_code int
)
DETERMINISTIC
begin
delete from u_cl_tb where U_code = p_u_code && u_cl_code = p_u_cl_code;
end $$




