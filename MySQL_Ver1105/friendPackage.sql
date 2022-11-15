# friend 프로시저 및 함수

# create friend data procedure
# id 또는 phone을 통해서 친구 추가
# id, phone 둘 다 들어오거나 id, phone 둘 중 하나는 반드시 들어와야 함
DELIMITER $$
CREATE procedure create_friend(p_U_code int,p_fr_id varchar(32),p_fr_phone varchar(15))
DETERMINISTIC
BEGIN
	declare p_fr_nick varchar(32);
	declare p_fr_code int;
	if(p_fr_id = '' or p_fr_id IS NULL) then
		select U_nick, U_code into p_fr_nick, p_fr_code from user_tb where U_phone = p_fr_phone;
	elseif (p_fr_phone = '' or p_fr_phone IS NULL) then 
		select U_nick, U_code into p_fr_nick, p_fr_code from user_tb where U_id = p_fr_id;
	else
		select U_nick, U_code into p_fr_nick, p_fr_code from user_tb where U_id = p_fr_id;
	end if;
	insert into friend_tb (U_code, fr_code, fr_nick) 
	values (p_U_code, p_fr_code, p_fr_nick);
END $$

# update firend data procedure
# 친구의 별명, 숨김, 차단을 설정
# 별명, 숨김, 차단 중 하나는 반드시 들어오야 함
DELIMITER $$
CREATE procedure update_friend (p_U_code int, p_fr_code int, p_nick varchar(32), p_hide bool, p_block bool, p_is_deleted int)
DETERMINISTIC
BEGIN
	if ((p_nick != '') and (p_nick IS NOT NULL)) then 
		update friend_tb set fr_nick = p_nick where U_code = p_u_code and fr_code = p_fr_code;
	end if;
	if (p_hide IS NOT NULL) then
		update friend_tb set fr_hide = p_hide where U_code = p_u_code and fr_code = p_fr_code;
	end if;
	if (p_block IS NOT NULL) then
		update friend_tb set fr_block = p_block where U_code = p_u_code and fr_code = p_fr_code;
	end if;
    if (p_is_deleted IS NOT NULL) then
		update friend_tb set is_deleted = p_is_deleted where U_code = p_u_code and fr_code = p_fr_code;
	end if;
END $$

# select friend list procedure
# 친구 정보를 확인
# friend_code가 null 일경우 user의 모든 data 가져오고 그 외의 경우 특정 friend의 data를 가져옴
DELIMITER $$
create procedure select_friend (p_U_code int,p_fr_code int)
DETERMINISTIC
BEGIN
	if(p_fr_code IS NULL) then
		select fr_code,  fr_nick, fr_hide, fr_block, is_deleted from friend_tb where u_code = p_U_code;
	else 
		select fr_code,  fr_nick, fr_hide, fr_block, is_deleted from friend_tb where u_code = p_U_code and fr_code = p_fr_code ;
	end if;
END $$