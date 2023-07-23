-- Mameクラス
local Mame = { }
Mame.new = function(edamame_index, mame_index)
	local instance = { }
	instance.__edamame_index = edamame_index
	instance.__index = mame_index
	instance.__mame = nil
	instance.__position = Vector3.zero
	instance.__out_flag = false

	instance.initialise = function(self)
		-- 豆の座標取得
		self.__mame = vci.assets.GetTransform("Mame (" .. self.__edamame_index .. "-" .. self.__index .. ")")
		self.__position = self.__mame.GetLocalPosition( )
	end

	instance.out = function(self)
		-- 豆が押し出される
		self.__position.z = self.__position.z + 0.028
		self.__mame.SetLocalPosition(self.__position)
    		self.__mame.SetVelocity(Vector3.zero)
		self.__out_flag = true
	end

	instance.eaten = function(self)
		-- 豆が食べられる
		if self.__out_flag then
			self.__position.y = self.__position.y + 2000
			self.__mame.SetLocalPosition(self.__position)
			self.__mame.SetVelocity(Vector3.zero)
		end
	end

	instance.go_back = function(self)
		-- 何かしらで元の位置に戻る
		self.__position.y = self.__position.y - 2000
		self.__mame.SetLocalPosition(self.__position)
    		self.__mame.SetVelocity(Vector3.zero)
		self.__out_flag = false
	end


	return instance
end
return Mame
