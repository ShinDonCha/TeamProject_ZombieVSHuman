<?php
	$u_id = $_POST["Input_user"];
	$u_pw = $_POST["Input_pass"];
	$nick = $_POST["Input_nick"];

	if ( empty( $u_id ) ) 
		die("u_id is empty. \n");

	if ( empty( $u_pw ) ) 
		die("u_pw is empty. \n");

	if ( empty( $nick ) ) 
		die("nick is empty. \n");

	$con = mysqli_connect("localhost", "zombiehuman", "zhcompany#0", "zombiehuman");
		//"localhost" <-- 같은 서버 내 라는 뜻

	if(!$con)
		die("Could not Connect".mysqli_connect_error());
		//연결 실패했을 경우 이 스크립트를 닫아주겠다는 뜻

	$check = mysqli_query($con, "SELECT user_id FROM ZombievsHuman WHERE user_id = '".$u_id."'");	    //user_id는 닷홈에 저장해둔 변수
	$numrows = mysqli_num_rows($check);		//0이 아니면 같은 아이디가 있는것
	if($numrows != 0)
	{
		die("ID does exist. \n");
	}

	$check = mysqli_query($con, "SELECT nick_name FROM ZombievsHuman WHERE nick_name = '".$nick."'");	//nick_name은 닷홈에 저장해둔 변수
	$numrows = mysqli_num_rows($check);
	if($numrows != 0)	//즉 0이 아니라는 뜻은 내가 찾는 Nickname 값이 존재한다는 뜻이다.
	{
		die("Nickname does exist. \n");
	}
	
	$Result = mysqli_query($con, "INSERT INTO ZombievsHuman (user_id, user_pw, nick_name) VALUES ('".$u_id."', '".$u_pw."', '".$nick."');");
	//여기(닷홈에 만든 변수들)에 뒤의 변수들의 값을 넣어줌 (새로운 아이디 생성하는 부분)

	if($Result)
		echo "Create Success. \n";
	else
		echo "Create error. \n";	
	
	mysqli_close($con);
?>