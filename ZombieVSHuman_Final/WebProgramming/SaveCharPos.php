<?php
	$u_id = $_POST["Input_user"];		//스트립트에 있는 키값으로 들어온 값을 받아옴
	$CharPos = $_POST["Char_pos"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");

	if (!$con) 
		die("Could not Connect".mysqli_connect_error());
	//연결 실패했을 경우 이 스크립트를 닫아주겠다는 듯


	$check = mysqli_query($con, "SELECT * FROM ZombievsHuman WHERE user_id = '". $u_id ."'");

	$numrows = mysqli_num_rows($check);
	if($numrows == 0)
	{ //mysqli_num_rows() 함수는 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼 때 쓰임
	  //즉 0 이라는 뜻은 해당 조건을 못 찾았다는 뜻
		die("ID does not exist. \n");
	}
	
	$row = mysqli_fetch_assoc($check);	//user_id 이름에 해당하는 행의 내용을 가져온다.
	if($row)	
	{
		mysqli_query($con, "UPDATE ZombievsHuman SET `CharPos` = '".$CharPos."'

		WHERE `user_id` = '".$u_id."' ");		
		
		echo ("SaveSuccess~");
	}
	mysqli_close($con);
?>