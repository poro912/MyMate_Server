# 네이밍
# 1. 프로시저 및 함수의 매개변수는 접두어를 사용하여 구분
# 1-1. 사용되는 방법에 따라 변수는 접두어를 사용하여 구분
# (첫글자 소문자로 사용)
# p_ : 프로시저 매개변수
# f_ : 함수 매개변수
# s_ : select한 변수
# i_ : insert할 변수
# 2. 매개변수가 칼럼명일 경우 동일하게 사용
# ex) user_tb의 U_id에 입력할 매개변수 명은 p_U_code
# 3. 테이블 명 및 기타 요소들 명칭 그대로 사용
# (user_tb인 테이블을 User_tb, user_Tb등으로 사용하지 않기)

# user 프로시저 및 함수

# create user data procedure
DELIMITER $$
CREATE procedure create_user(
p_U_id varchar(32),
p_U_pwd varchar(32),
p_U_nick varchar(32),
p_U_name varchar(32),
p_U_phone varchar(15),
p_U_email varchar(50)
)
begin
	insert into user_tb values((select get_seq('user')), p_U_id, p_U_pwd, p_U_nick, p_U_name, p_U_phone, p_U_email, null, null, false);
END $$

# user code select procedure
DELIMITER $$
create procedure select_U_code(p_U_id varchar(32))
DETERMINISTIC
BEGIN
	select U_code from user_tb where U_id = p_U_id;
END $$ 

# user info select procedure
# data_format values
# '' or null : all colms
#  0 : U_id colm 
#  1 : U_nickname colm 
#  2 : U_name colm 
#  3 : U_phone colm 
#  4 : U_email colm 
DELIMITER $$
CREATE procedure select_user (p_U_code int, p_data_format varchar(32))
DETERMINISTIC
BEGIN
	if ((p_U_code IS NULL or p_U_code = 0) and (p_data_format IS NULL or p_data_format = "")) then select U_id, U_nick , U_name , U_phone, U_email, U_content, is_deleted  from user_tb;
	elseif ((p_data_format IS NULL) or (p_data_format = "")) then select U_id, U_nick , U_name , U_phone, U_email, U_content, is_deleted  from user_tb where U_code = p_U_code;
    elseif (p_data_format = "id") then select U_id from user_tb where U_code = p_U_code;
	elseif (p_data_format = "pwd") then select U_pwd from user_tb where U_code = p_U_code;
    elseif (p_data_format = "nick") then select U_nick from user_tb where U_code = p_U_code;
	elseif (p_data_format = "name") then select U_name from user_tb where U_code = p_U_code;
	elseif (p_data_format = "phone") then select U_phone from user_tb where U_code = p_U_code;
	elseif (p_data_format = "email") then select U_email  from user_tb where U_code = p_U_code;
    elseif (p_data_format = "content") then select U_content  from user_tb where U_code = p_U_code;
    elseif (p_data_format = "recent_time") then select U_recent_time  from user_tb where U_code = p_U_code;
    elseif (p_data_format = "is_deleted") then select is_deleted  from user_tb where U_code = p_U_code;
	end if;
END $$

# user info update procedure
# colm 값이 null값이나 공백으로 입력하면 해당 칼럼은 업데이트 미실시
DELIMITER $$
CREATE procedure update_user (p_U_code int, p_U_pwd varchar(32), p_U_nick varchar(32), p_U_name varchar(32) , p_U_phone varchar(15), p_U_email varchar(50), p_U_content varchar(500),  p_is_deleted bool)
DETERMINISTIC
BEGIN
	if (p_U_pwd IS NOT NULL) then update user_tb set U_pwd = p_U_pwd where U_code= p_U_code;
	end if;
	if ((p_U_nick IS NOT NULL) and (p_U_nick != '')) then update user_tb set U_nick = p_U_nick where U_code = p_U_code;
	end if;
	if ((p_U_name IS NOT NULL) and (p_U_name != '')) then update user_tb set U_name = p_U_name where U_code = p_U_code;
	end if;
	if ((p_U_phone IS NOT NULL) and (p_U_phone != '')) then update user_tb set U_phone = p_U_phone where U_code = p_U_code;
	end if;
	if ((p_U_email IS NOT NULL) and (p_U_email != '')) then update user_tb set U_email = p_U_email where U_code = p_U_code;
	end if;
	if ((p_U_content IS NOT NULL) and (p_U_content != '')) then update user_tb set U_content = p_U_content where U_code = p_U_code;
	end if;
	if (p_is_deleted IS NOT NULL) then update user_tb set is_deleted = p_is_deleted where U_code = p_U_code;
	end if;
END $$

#login
DELIMITER $$
CREATE FUNCTION login_user (f_U_id VARCHAR(32), f_U_pwd varchar(32))
RETURNS bool
DETERMINISTIC
BEGIN
	DECLARE login bit;
	Declare s_U_id varchar(32);
	Declare s_U_pwd varchar(32);
	set login = false;
	select U_id into s_U_id from user_tb where U_id = f_U_id;

	if s_U_id is null then
		return login;
	end if;

	select U_pwd into s_U_pwd from user_tb where U_id = f_U_id;

	if s_U_pwd = f_U_pwd then
		set login = true;
		update user_tb set U_recent_time = (select sysdate()) where U_id = f_U_id;
	end if;
    
return login;
END $$