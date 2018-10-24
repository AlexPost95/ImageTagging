create table Image (
ID		int		not null	primary key,
Picture	Image	not null
)

create table Tag (
PictureID	int		not null	foreign key references Image(ID),
Tag			varchar(255)	not null
)